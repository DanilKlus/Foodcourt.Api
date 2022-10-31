using Foodcourt.Data.Api.Entities.Cafes;
using Foodcourt.Data.Api.Entities.Users;

namespace Foodcourt.Data.Api.Entities.Orders;

public class Order
{
    public long Id { get; set; }
    public OrderStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public double TotalPrice { get; set; }
    public DateTime CreationTime { get; set; }
    public string Comment { get; set; }
    
    public string AppUserId { get; set; }
    public AppUser AppUser { get; set; }
    public long CafeId { get; set; }
    public Cafe Cafe { get; set; }
    public List<OrderProduct> OrderProducts { get; set; }
}