using Microsoft.EntityFrameworkCore;
using Estoque.API.Data;
using Estoque.API.Models;

namespace Estoque.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly EstoqueDbContext _context;
        public ProductRepository(EstoqueDbContext context)
        {
            _context = context;
        }

        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task DeleteAsync(int id)
        {
            var p = await _context.Products.FindAsync(id);
            if (p == null) return;
            _context.Products.Remove(p);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // 👇 Aqui está o método usado pelo RabbitMQConsumer
        public async Task UpdateStockAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new Exception($"Produto com ID {productId} não encontrado.");

            if (product.Quantity < quantity)
                throw new Exception($"Estoque insuficiente para o produto {product.Name}.");

            product.Quantity -= quantity;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
    }
}