

using System.ComponentModel.DataAnnotations.Schema;

namespace Foodcourt.Data.Api.Entities.Users;

public class Basket
{
    public long Id { get; set; }
    public BasketStatus Status { get; set; }

    [ForeignKey("AppUserId")]
    public string AppUserId { get; set; }
    public AppUser AppUser { get; set; }
    
   
    public List<BasketProduct> BasketProducts { get; set; }
}