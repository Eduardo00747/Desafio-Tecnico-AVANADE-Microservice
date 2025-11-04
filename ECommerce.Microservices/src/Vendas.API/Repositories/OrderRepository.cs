using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vendas.API.Data;
using Vendas.API.Models;

namespace Vendas.API.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly VendasDbContext _ctx;
        public OrderRepository(VendasDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Order> AddAsync(Order order)
        {
            _ctx.Orders.Add(order);
            await _ctx.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _ctx.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            // Carrega os pedidos com seus itens e ordena por data decrescente (mais recentes primeiro)
            return await _ctx.Orders
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task DeleteAllAsync()
        {
            _ctx.OrderItems.RemoveRange(_ctx.OrderItems);
            _ctx.Orders.RemoveRange(_ctx.Orders);
            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var order = await _ctx.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order != null)
            {
                // Remove primeiro os items do pedido
                _ctx.OrderItems.RemoveRange(order.Items);
                // Depois remove o pedido
                _ctx.Orders.Remove(order);
                await _ctx.SaveChangesAsync();
            }
        }
    }
}
