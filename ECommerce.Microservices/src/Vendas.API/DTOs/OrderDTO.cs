using System;
using System.Collections.Generic;

namespace Vendas.API.DTOs
{
    public class OrderDTO
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }
        public decimal Total { get; set; }
        public List<OrderItemResultDTO> Items { get; set; } = new();
    }

    public class OrderItemResultDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}