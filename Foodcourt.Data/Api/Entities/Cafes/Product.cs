using Foodcourt.Data.Api.Entities.Orders;
using Foodcourt.Data.Api.Entities.Users;

namespace Foodcourt.Data.Api.Entities.Cafes;

public class Product
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Avatar { get; set; }
    public ProductStatus Status { get; set; }
    public double Price { get; set; }
    
    
    internal virtual OrderProduct OrderProduct { get; set; }
    internal virtual BasketProduct BasketProduct { get; set; }
    
    public long CafeId { get; set; }
    internal Cafe Cafe { get; set; }
   
    public virtual List<ProductVariant> ProductVariants {get; set; }
    public virtual List<ProductType> ProductTypes {get; set; }
}