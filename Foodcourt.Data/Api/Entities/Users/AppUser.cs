using Foodcourt.Data.Api.Entities.Cafes;
using Foodcourt.Data.Api.Entities.Orders;
using Microsoft.AspNetCore.Identity;

namespace Foodcourt.Data.Api.Entities.Users;

public class AppUser : IdentityUser
{
    public string Name { get; set; }
    public string Password { get; set; }
    public bool IsActive { get; set; }
    
    public Basket Basket { get; set; } 
    
    public List<Order> Orders { get; set; }
    
    public virtual List<Role> Roles { get; set; }
    public virtual List<Cafe> Cafes { get; set; }
}