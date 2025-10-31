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
            return await _ctx.Orders
                .Include(o => o.Items)
                .ToListAsync();
        }

        public async Task DeleteAllAsync()
        {
            _ctx.OrderItems.RemoveRange(_ctx.OrderItems);
            _ctx.Orders.RemoveRange(_ctx.Orders);
            await _ctx.SaveChangesAsync();
        }
    }
}
