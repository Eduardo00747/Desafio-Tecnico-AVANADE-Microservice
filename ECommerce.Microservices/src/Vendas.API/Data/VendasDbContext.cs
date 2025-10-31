using Microsoft.EntityFrameworkCore;
using Vendas.API.Models;


namespace Vendas.API.Data
{
    public class VendasDbContext : DbContext
    {
        public VendasDbContext(DbContextOptions<VendasDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId);
        }
    }
}