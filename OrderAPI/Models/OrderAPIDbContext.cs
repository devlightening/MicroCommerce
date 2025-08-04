
using Microsoft.EntityFrameworkCore;

namespace OrderAPI.Models
{
    public class OrderAPIDbContext : DbContext
    {
        public OrderAPIDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<OrderAPI.Models.Entites.Order> Orders { get; set; }
        public DbSet<OrderAPI.Models.Entites.OrderItem> OrderItems { get; set; }


    }
}
