using Microsoft.EntityFrameworkCore;
using Gateway.API.Models;

namespace Gateway.API.Data
{
    public class GatewayDbContext : DbContext
    {
        public GatewayDbContext(DbContextOptions<GatewayDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Password).IsRequired();
                entity.HasIndex(e => e.Username).IsUnique();
            });
        }
    }
}