using System;
using System.Collections.Generic;

namespace Vendas.API.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string CustomerId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public decimal Total { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        public string Status { get; set; } = "Created";
    }
}