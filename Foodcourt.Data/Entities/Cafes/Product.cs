using Newtonsoft.Json;

namespace Foodcourt.Data.Entities.Cafes;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Avatar { get; set; }
    public ProductStatus Status { get; set; }
    public double Price { get; set; }
    
    public Guid CafeId { get; set; }
    public List<ProductVariant> ProductVariants {get; set; }
    public List<ProductType> ProductType {get; set; }
}