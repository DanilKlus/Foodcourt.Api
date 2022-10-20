namespace Foodcourt.Data.Entities.Users;

public class Basket
{
    public Guid Id { get; set; }
    public BasketStatus Status { get; set; }

    public Guid UserId { get; set; }
    public List<BasketProduct> BasketProducts { get; set; }
}