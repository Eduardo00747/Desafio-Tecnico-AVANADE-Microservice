using Estoque.API.Models;

namespace Estoque.API.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<Product> AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);

        // 👇 Adicione este novo método
        Task UpdateStockAsync(int productId, int quantity);
    }
}