using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Vendas.API.DTOs;
using Vendas.API.Models;
using Vendas.API.Repositories;
using Vendas.API.Services;

namespace Vendas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _repo;
        private readonly IEstoqueClient _estoque;
        private readonly IRabbitMQService _rabbit;

        public OrdersController(IOrderRepository repo, IEstoqueClient estoque, IRabbitMQService rabbit)
        {
            _repo = repo;
            _estoque = estoque;
            _rabbit = rabbit;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateOrderDTO dto)
        {
            // Validação básica
            if (dto.Items == null || !dto.Items.Any())
                return BadRequest("Pedido deve conter ao menos 1 item.");

            // Captura o token JWT que veio no cabeçalho Authorization
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            decimal total = 0m;

            // Captura o nome do usuário a partir do claim correto do JWT
            var userName = User.Claims.FirstOrDefault(c =>
                c.Type == "name" ||
                c.Type == "sub" ||
                c.Type == "email" ||
                c.Type == "Username" ||
                c.Type == "unique_name")?.Value ?? "Usuário Desconhecido";

            var order = new Order { CustomerId = userName };

            foreach (var it in dto.Items)
            {
                // 🔹 Agora passamos o token para o EstoqueClient
                var prod = await _estoque.GetProductAsync(it.ProductId, token);

                if (prod is null)
                    return BadRequest($"Produto {it.ProductId} não encontrado no estoque.");

                if (prod.Quantity < it.Quantity)
                    return BadRequest($"Estoque insuficiente para o produto {prod.Name ?? "Desconhecido"} (disponível {prod.Quantity}).");

                var productName = prod.Name ?? "Desconhecido";
                var price = prod.Price;

                var newQuantity = prod.Quantity - it.Quantity;

                // 🔹 Atualiza o estoque remoto no Estoque.API
                var updated = await _estoque.UpdateProductQuantityAsync(it.ProductId, newQuantity, token);
                if (!updated)
                    return StatusCode(500, $"Erro ao atualizar estoque do produto {prod.Name}.");

                order.Items.Add(new OrderItem
                {
                    ProductId = prod.Id,
                    ProductName = productName,
                    UnitPrice = price,
                    Quantity = it.Quantity
                });

                total += price * it.Quantity;
            }

            order.Total = total;

            // Salvar pedido
            var created = await _repo.AddAsync(order);

            // Publicar evento RabbitMQ
            var createdItems = created.Items ?? new List<OrderItem>();
            var eventPayload = new
            {
                OrderId = created.Id,
                Items = createdItems.Select(i => new { i.ProductId, i.Quantity })
            };

            _rabbit.PublishOrderCreated(eventPayload);

            // Retornar DTO
            var resultItems = createdItems.Select(i => new OrderItemResultDTO
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName ?? "Desconhecido",
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            }).ToList();

            var result = new OrderDTO
            {
                Id = created.Id,
                CreatedAt = created.CreatedAt,
                Total = created.Total,
                Items = resultItems
            };

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _repo.GetByIdAsync(id);

            if (order is null)
            {
                return NotFound(new
                {
                    message = "Produto não encontrado no carrinho, adicione um produto ou coloque o ID correspondente ao produto cadastrado."
                });
            }

            var orderItems = order.Items ?? new List<OrderItem>(); // 👈 protege contra null

            var dto = new OrderDTO
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                Total = order.Total,
                Items = orderItems.Select(i => new OrderItemResultDTO
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName ?? "Desconhecido",
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity
                }).ToList()
            };

            return Ok(dto);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _repo.GetAllAsync();

            if (orders == null || !orders.Any())
            {
                return NotFound(new
                {
                    message = "Nenhum produto foi adicionado ainda. Adicione um produto para visualizar os pedidos."
                });
            }

            var result = orders.Select(order => new OrderDTO
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                Total = order.Total,
                Items = (order.Items ?? new List<OrderItem>()).Select(i => new OrderItemResultDTO
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName ?? "Desconhecido",
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity
                }).ToList()
            }).ToList();

            return Ok(result);
        }

        [HttpDelete("all")]
        [Authorize]
        public async Task<IActionResult> DeleteAll()
        {
            await _repo.DeleteAllAsync();
            return Ok(new { message = "Todos os pedidos foram excluídos com sucesso." });
        }
    }
}
