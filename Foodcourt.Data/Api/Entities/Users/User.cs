using Foodcourt.Data.Api.Entities.Cafes;
using Foodcourt.Data.Api.Entities.Orders;

namespace Foodcourt.Data.Api.Entities.Users;

public class User
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Password { get; set; }
    public bool IsActive { get; set; }
    
    public virtual Basket Basket { get; set; } 
    
    public List<Order> Orders { get; set; }
    
    public virtual List<Role> Roles { get; set; }
    public virtual List<Cafe> Cafes { get; set; }
}