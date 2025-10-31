
namespace Vendas.API.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; } // id do produto no Estoque
        public string? ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        public int OrderId { get; set; }
        public Order? Order { get; set; }
    }
}