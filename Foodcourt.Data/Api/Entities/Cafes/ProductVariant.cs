using Newtonsoft.Json;

namespace Foodcourt.Data.Entities.Cafes;

public class ProductVariant
{
    public long Id { get; set; }
    public string Variant { get; set; }
    
    public List<Product> Products { get; set; }
}