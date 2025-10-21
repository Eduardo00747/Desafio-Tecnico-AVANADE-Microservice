using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Estoque.API.DTOs
{
    public class ProductDTO
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Price { get; set; } = string.Empty;

        public int Quantity { get; set; }
    }
}