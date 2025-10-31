using System.Collections.Generic;

namespace Vendas.API.DTOs
{
    public class OrderItemDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}