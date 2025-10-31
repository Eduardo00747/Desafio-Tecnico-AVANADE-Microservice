using System.Collections.Generic;

namespace Vendas.API.DTOs
{
    public class CreateOrderDTO
    {
        public List<OrderItemDTO> Items { get; set; } = new();

    }
}