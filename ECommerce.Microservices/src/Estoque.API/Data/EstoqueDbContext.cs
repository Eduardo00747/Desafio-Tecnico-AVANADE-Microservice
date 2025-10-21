using Microsoft.EntityFrameworkCore;
using Estoque.API.Models;


namespace Estoque.API.Data
{
    public class EstoqueDbContext : DbContext
    {
        public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } 

        public DbSet<Product> Products { get; set; } = null!;

    }
}