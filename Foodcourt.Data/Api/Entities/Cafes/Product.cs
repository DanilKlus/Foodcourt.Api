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
    
    
    public virtual OrderProduct OrderProduct { get; set; }
    public virtual BasketProduct BasketProduct { get; set; }
    
    public long CafeId { get; set; }
    public Cafe Cafe { get; set; }
   
    public virtual List<ProductVariant>? ProductVariants {get; set; }
    public virtual List<ProductType>? ProductTypes {get; set; }
}