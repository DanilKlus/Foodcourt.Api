namespace Foodcourt.BusinessLogic.Services.Orders;

public interface IOrderService
{
    Task CreateOrders(string userId);
}