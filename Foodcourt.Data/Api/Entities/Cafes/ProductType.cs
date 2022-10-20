namespace Foodcourt.Data.Entities.Cafes;

public class ProductType
{
    public long Id { get; set; }
    public string Type { get; set; }
    
    public List<Product> Products { get; set; }
}