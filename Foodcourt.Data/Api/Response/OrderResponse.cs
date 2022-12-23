using Foodcourt.Data.Api.Entities.Orders;

namespace Foodcourt.Data.Api.Response;

public class OrderResponse
{
    public long Id { get; set; }
    public OrderStatus Status { get; set; }
    public double TotalPrice { get; set; }
    public DateTime CreationTime { get; set; }
    public string Comment { get; set; }
    public string CafeName { get; set; }
    public long CafeId { get; set; }
    public List<OrderProductResponse>? Products { get; set; }
}

public class OrderProductResponse : ProductResponse
{
    public long ProductId { get; set; }
    public int Count { get; set; }
    public ProductVariantResponse ProductVariants { get; set; }
}