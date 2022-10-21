namespace Foodcourt.Data.Api.Entities.Cafes;

public class ProductVariant
{
    public long Id { get; set; }
    public string Variant { get; set; }
    
    public virtual List<Product> Products { get; set; }
}