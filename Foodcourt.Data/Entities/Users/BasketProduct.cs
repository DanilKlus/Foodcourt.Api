using Foodcourt.Data.Entities.Cafes;
using Newtonsoft.Json;

namespace Foodcourt.Data.Entities.Users;

public class BasketProduct
{
    public Guid Id { get; set; }
    public int Count { get; set; }
    

    public Guid BasketId { get; set; }
    public Product Product { get; set; }
    public ProductVariant ProductVariant { get; set; }
}