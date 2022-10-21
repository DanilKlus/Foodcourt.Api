namespace Foodcourt.Data.Api.Entities.Cafes;

public class ProductType
{
    public ProductType(long id, string type)
    {
        Id = id;
        Type = type;
    }

    public long Id { get; set; }
    public string Type { get; set; }
    
    public virtual List<Product> Products { get; set; }
}