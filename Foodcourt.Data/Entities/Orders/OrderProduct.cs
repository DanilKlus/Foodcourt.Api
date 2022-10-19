using Foodcourt.Data.Entities.Cafes;
using Newtonsoft.Json;

namespace Foodcourt.Data.Entities.Orders;

public class OrderProduct
{
    public long Id { get; set; }
    public int Count { get; set; }
    
    public long OrderId { get; set; }
    public Product Product { get; set; }
    public ProductVariant ProductVariant { get; set; }
}