using Foodcourt.Data.Api.Entities.Cafes;
using Foodcourt.Data.Api.Entities.Orders;
using Foodcourt.Data.Api.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Foodcourt.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseSerialColumns();
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Cafe> Cafes { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Basket> Baskets { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductType> ProductTypes { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<OrderProduct> OrderProducts { get; set; }
    public DbSet<BasketProduct> BasketProducts { get; set; }
}
