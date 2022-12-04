using Foodcourt.Data.Api;
using Foodcourt.Data.Api.Entities.Orders;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;

namespace Foodcourt.BusinessLogic.Services.Orders;

public interface IOrderService
{
    Task CreateOrdersAsync(string userId);
    Task<SearchResponse<OrderResponse>> GetOrdersAsync(string userId, OrderStatus? orderStatus, SearchRequest searchRequest);
    Task<OrderResponse> GetOrderAsync(string userId, long orderId);
    Task CancelOrderAsync(string userId, long orderId);
    Task<OrderResponse> PatchOrderAsync(string userId, long orderId, PathOrderRequest patchRequest);
    Task RepeatOrderAsync(string userId, long orderId);
    Task PayOrdersAsync(string userId, List<long> ordersIds);
}