using System.ComponentModel.DataAnnotations.Schema;
using Foodcourt.Data.Api.Entities.Cafes;

namespace Foodcourt.Data.Api.Entities.Users;

public class BasketProduct
{
    public long Id { get; set; }
    public int Count { get; set; }
    
    
    [ForeignKey("ProductId")]
    public long ProductId { get; set; }
    public virtual Product Product { get; set; }
    
    [ForeignKey("ProductVariantId")]
    public long ProductVariantId { get; set; }
    public virtual ProductVariant ProductVariant { get; set; }
    
    public long BasketId { get; set; }
    public Basket Basket { get; set; }
    
}