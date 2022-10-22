using Foodcourt.Data.Api.Entities.Cafes;
using Foodcourt.Data.Api.Entities.Orders;
using Foodcourt.Data.Api.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Foodcourt.Data;

public class AppDataContext : IdentityDbContext<IdentityUser>
{
    public AppDataContext(DbContextOptions<AppDataContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
    
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Cafe> Cafes { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Basket> Baskets { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductType> ProductTypes { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<OrderProduct> OrderProducts { get; set; }
    public DbSet<BasketProduct> BasketProducts { get; set; }
}
