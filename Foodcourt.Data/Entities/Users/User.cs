using System.ComponentModel.DataAnnotations;
using Foodcourt.Data.Entities.Cafes;
using Foodcourt.Data.Entities.Orders;

namespace Foodcourt.Data.Entities.Users;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Password { get; set; }
    public bool IsActive { get; set; }
    
    public Basket Basket { get; set; } 
    public List<Order> Orders { get; set; }
    public List<Role> Roles { get; set; }
    public List<Cafe> Cafes { get; set; }
}