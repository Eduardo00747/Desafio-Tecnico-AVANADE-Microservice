using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Estoque.API.Models;
using Estoque.API.Repositories;
using Estoque.API.DTOs;
using System.Globalization;
using System.Text.Json;

namespace Estoque.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // protege todos endpoints de produto
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repo;
        public ProductsController(IProductRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _repo.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductDTO dto)
        {
            // Verifica se o preço é nulo ou vazio
            if (string.IsNullOrWhiteSpace(dto.Price))
                return BadRequest("❌ O campo preço é obrigatório.");

            // Verifica se contém vírgula ou ponto
            if (dto.Price.Contains(',') || dto.Price.Contains('.'))
                return BadRequest("❌ Valor inválido. Não use ponto ou vírgula para cadastrar o preço do produto. Exemplo: 4000, 5500, 100.");

            // Verifica se é um número inteiro válido
            if (!int.TryParse(dto.Price, out int priceValue))
                return BadRequest("❌ Valor inválido. O preço deve conter apenas números inteiros. Exemplo: 500, 1500, 7000.");

            // Verifica se o preço é negativo
            if (priceValue < 0)
                return BadRequest("❌ Valor inválido. O preço não pode ser negativo.");

            // 🔹 Validação da quantidade
            var quantityStr = dto.Quantity.ToString();

            if (quantityStr.Contains(',') || quantityStr.Contains('.'))
                return BadRequest("❌ Quantidade inválida. Não use ponto ou vírgula. Exemplo: 10, 50, 100.");

            if (dto.Quantity < 0)
                return BadRequest("❌ Quantidade inválida. O valor não pode ser negativo.");

            // Cria o objeto Product usando o valor convertido
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = priceValue, // converte de int para decimal automaticamente
                Quantity = dto.Quantity
            };

            var created = await _repo.AddAsync(product);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDTO dto)
        {

            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return NotFound("❌ Produto não encontrado.");

            // Verifica se o preço é nulo ou vazio
            if (string.IsNullOrWhiteSpace(dto.Price))
                return BadRequest("❌ O campo preço é obrigatório.");

            // Verifica se contém vírgula ou ponto
            if (dto.Price.Contains(',') || dto.Price.Contains('.'))
                return BadRequest("❌ Valor inválido. Não use ponto ou vírgula para cadastrar o preço do produto. Exemplo: 4000, 5500, 100.");

            // Verifica se é um número inteiro válido
            if (!int.TryParse(dto.Price, out int priceValue))
                return BadRequest("❌ Valor inválido. O preço deve conter apenas números inteiros. Exemplo: 500, 1500, 7000.");

            // Verifica se o preço é negativo
            if (priceValue < 0)
                return BadRequest("❌ Valor inválido. O preço não pode ser negativo.");

            // 🔹 Validação da quantidade
            var quantityStr = dto.Quantity.ToString();

            if (quantityStr.Contains(',') || quantityStr.Contains('.'))
                return BadRequest("❌ Quantidade inválida. Não use ponto ou vírgula. Exemplo: 10, 50, 100.");

            if (dto.Quantity < 0)
                return BadRequest("❌ Quantidade inválida. O valor não pode ser negativo.");


            // Atualiza apenas os campos necessários
            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Price = priceValue; // conversão automática para decimal
            existing.Quantity = dto.Quantity;

            await _repo.UpdateAsync(existing);
            return Ok("✅ Produto atualizado com sucesso!");
        }

        [HttpPut("{id:int}/quantity")]
        public async Task<IActionResult> UpdateQuantity(int id, [FromBody] UpdateQuantityDTO dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return NotFound("❌ Produto não encontrado.");

            // Validação da quantidade
            if (dto.Quantity < 0)
                return BadRequest("❌ Quantidade inválida. O valor não pode ser negativo.");

            // Atualiza apenas a quantidade
            existing.Quantity = dto.Quantity;

            await _repo.UpdateAsync(existing);
            return Ok("✅ Quantidade atualizada com sucesso!");
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();
            await _repo.DeleteAsync(id);
            return NoContent();
        }
    }
}