

using System.ComponentModel.DataAnnotations.Schema;

namespace Foodcourt.Data.Api.Entities.Users;

public class Basket
{
    public long Id { get; set; }
    public BasketStatus Status { get; set; }

    [ForeignKey("UserId")]
    public long UserId { get; set; }
    
    public virtual User User { get; set; }
    public List<BasketProduct> BasketProducts { get; set; }
}