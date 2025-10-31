using System.Threading.Tasks;

namespace Vendas.API.Services
{
    public interface IEstoqueClient
    {
        Task<ProductResponse?> GetProductAsync(int productId, string? token = null);
        Task<bool> UpdateProductQuantityAsync(int id, int newQuantity, string token);        
    }

    public class ProductResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}