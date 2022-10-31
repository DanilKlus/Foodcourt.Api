using Foodcourt.Data.Api;
using Foodcourt.Data.Api.Entities.Orders;
using Foodcourt.Data.Api.Response;

namespace Foodcourt.BusinessLogic.Services.Orders;

public interface IOrderService
{
    Task CreateOrders(string userId);
    Task<SearchResponse<OrderResponse>> GetOrders(string userId, OrderStatus? orderStatus);
}