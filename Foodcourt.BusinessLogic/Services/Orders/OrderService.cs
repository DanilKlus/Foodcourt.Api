using Foodcourt.BusinessLogic.Extensions;
using Foodcourt.Data;
using Foodcourt.Data.Api;
using Foodcourt.Data.Api.Entities.Orders;
using Foodcourt.Data.Api.Entities.Users;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Foodcourt.Data.Api.Response.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Foodcourt.BusinessLogic.Services.Orders;

public class OrderService : IOrderService
{
    private readonly AppDataContext _dataContext;
    public OrderService(AppDataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task CreateOrdersAsync(string userId)
    {
        var basket = await _dataContext.Baskets.FirstOrDefaultAsync(x => x.AppUserId.Equals(userId));
        if (basket == null)
            throw new Exception($"unhandled error when get user: {userId} basket (user basket is null)");
        var basketProducts = await _dataContext.BasketProducts
            .Include(x => x.ProductVariant)
            .Include(p => p.Product)
                .ThenInclude(c => c.Cafe)
            .Where(product => product.BasketId.Equals(basket.Id))
            .ToListAsync();

        var orders = basketProducts
            .DistinctBy(x => x.Product.CafeId)
            .Select(x => new Order
            {
                Status = OrderStatus.Created,
                PaymentStatus = PaymentStatus.Created,
                TotalPrice = basketProducts
                    .Where(p => p.Product.CafeId.Equals(x.Product.CafeId))
                    .Select(p => p.Product.Price * p.Count).Sum(),
                CreationTime = DateTime.UtcNow,
                Comment = "",
                CafeName = x.Product.Cafe.Name,
                AppUserId = userId,
                CafeId = x.Product.CafeId,
                OrderProducts = basketProducts
                    .Where(p => p.Product.CafeId.Equals(x.Product.CafeId))
                    .Select(p => p.ToOrder()).ToList()
            }).ToList();
        basket.Status = BasketStatus.Empty;
        
        _dataContext.Orders.AddRange(orders); 
        _dataContext.BasketProducts.RemoveRange(basketProducts);
        _dataContext.Baskets.Update(basket);
        await _dataContext.SaveChangesAsync();
    }
    
    public async Task<OrderResponse> PatchOrderAsync(string userId, long orderId, PathOrderRequest patchRequest)
    {
        var order = await GetOrderEntityAsync(userId, orderId);
        order.Comment = patchRequest.Comment;
        _dataContext.Orders.Update(order);
        await _dataContext.SaveChangesAsync();
        return order.ToEntity();
    }

    public async Task RepeatOrderAsync(string userId, long orderId)
    {
        //TODO: refactor and test
        var basket = await _dataContext.Baskets.FirstOrDefaultAsync(x => x.AppUserId.Equals(userId));
        if (basket == null)
            throw new Exception($"unhandled error when get user: {userId} basket (user basket is null)");
        
        var order = await GetOrderEntityAsync(userId, orderId);
        var products = order.OrderProducts.Select(x => new BasketProduct
        {
            Count = x.Count, 
            BasketId = basket.Id,
            ProductId = x.ProductId, 
            ProductVariantId = x.ProductVariantId,
        }).ToList();

        basket.Status = BasketStatus.NotEmpty;
        _dataContext.Baskets.Update(basket);
        _dataContext.BasketProducts.AddRange(products);
        await _dataContext.SaveChangesAsync();
    }

    public async Task PayOrdersAsync(string userId, List<long> ordersIds)
    {
        var orders = await _dataContext.Orders.Where(x => x.AppUserId.Equals(userId) && ordersIds.Contains(x.Id)).ToListAsync();
        
        var payedOrders = new List<Order>();
        foreach (var order in orders)
        {
            order.Status = OrderStatus.InQueue;
            order.PaymentStatus = PaymentStatus.Paid;
            payedOrders.Add(order);
        }
        
        _dataContext.Orders.UpdateRange(payedOrders);
        await _dataContext.SaveChangesAsync();
    }

    public async Task<SearchResponse<OrderResponse>> GetOrdersAsync(string userId, OrderStatus? orderStatus, SearchRequest searchRequest)
    {
        var skipCount = searchRequest.Skip ?? 0;
        var takeCount = searchRequest.Take ?? 50;
        var ordersResult = await _dataContext.Orders.Where(x => x.AppUserId == userId).ToListAsync();
        var orders = (orderStatus != null ? ordersResult.Where(x => x.Status == orderStatus) : ordersResult)
            .Skip(skipCount).Take(takeCount).ToList();

        return new SearchResponse<OrderResponse>
        {
            FoundEntities = orders.Select(x => x.ToEntity()).ToList(),
            TotalCount = orders.Count
        };
    }

    public async Task<OrderResponse> GetOrderAsync(string userId, long orderId)
    {
        var order = await GetOrderEntityAsync(userId, orderId);
        return order.ToEntity();
    }

    public async Task CancelOrderAsync(string userId, long orderId)
    {
        var order = await _dataContext.Orders.FirstOrDefaultAsync(x => x.AppUserId == userId && x.Id.Equals(orderId));
        if (order == null) return;

        if (order.Status is OrderStatus.Created or OrderStatus.InQueue)
            order.Status = OrderStatus.Cancelled;
        else
            throw new CancelOrderException("Can't cancel order with current status", order.Status); 
        
        _dataContext.Orders.Update(order);
        await _dataContext.SaveChangesAsync();
    }

    private async Task<Order> GetOrderEntityAsync(string userId, long orderId)
    {
        var order = await _dataContext.Orders
            .Include(p => p.OrderProducts)
            .ThenInclude(t => t.ProductVariant)
            .Include(p => p.OrderProducts)
            .ThenInclude(t => t.Product)
            .FirstOrDefaultAsync(x => x.AppUserId == userId && x.Id.Equals(orderId));
        if (order == null)
            throw new NotFoundException($"order {orderId} not found");
        return order;
    }
}