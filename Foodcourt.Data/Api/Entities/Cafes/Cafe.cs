using Foodcourt.Data.Api.Entities.Orders;
using Foodcourt.Data.Api.Entities.Users;

namespace Foodcourt.Data.Api.Entities.Cafes;

public class Cafe
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public CafeStatus Status { get; set; }
    public string Avatar { get; set; }
    public string Location { get; set; }
    public bool IsActive { get; set; }
    
    internal List<Order> Orders { get; set; }
    internal List<Product> Products { get; set; }
    
    internal virtual List<User> Users { get; set; } 
    
    public string CertifyingDocument { get; set; }
    public string Response { get; set; }
    public string PersonalAccount { get; set; }
    
}