
namespace Foodcourt.Data.Entities.Orders;

public class Order
{
    public long Id { get; set; }
    public OrderStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public int TotalPrice { get; set; }
    public DateTime CreationTime { get; set; }
    public string Comment { get; set; }
    
    public Guid UserId { get; set; }
    public Guid CafeId { get; set; }
    public List<OrderProduct> OrderProducts { get; set; }
}