using Foodcourt.Data.Entities;
using Foodcourt.Data.Entities.Cafes;
using Foodcourt.Data.Entities.Orders;
using Foodcourt.Data.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Foodcourt.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) => 
        modelBuilder.UseSerialColumns();

    public DbSet<User> Users { get; set; }
    
    public DbSet<Cafe> Cafes { get; set; }

    public DbSet<Order> Orders { get; set; }
}
