using Foodcourt.Data.Api.Entities.Cafes;
using Foodcourt.Data.Api.Entities.Users;

namespace Foodcourt.Data.Api.Response;

public class BasketResponse
{
    public double TotalPrice { get; set; }
    public int TotalProductsCount { get; set; }
    public BasketStatus Status { get; set; }
    
    public List<CafeBasket> CafesBaskets { get; set; }
}

public class CafeBasket
{
    public long Id { get; set; }
    public string Name { get; set; }
    public List<BasketProductResponse> Products { get; set; }
}

public class BasketProductResponse : ProductResponse
{
    public int Count { get; set; }
    public ProductVariantResponse ProductVariants { get; set; }
}