namespace Foodcourt.Data.Api.Response;

public class ProductVariantResponse
{
    public ProductVariantResponse(long id, string type)
    {
        Id = id;
        Type = type;
    }
    
    public long Id { get; set; }
    public string Type { get; set; }
}