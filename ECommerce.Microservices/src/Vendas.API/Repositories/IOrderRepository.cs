using System.Threading.Tasks;
using Vendas.API.Models;

namespace Vendas.API.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> AddAsync(Order order);
        Task<Order?> GetByIdAsync(int id);
        Task<IEnumerable<Order>> GetAllAsync();
        Task DeleteAllAsync();
    }
}