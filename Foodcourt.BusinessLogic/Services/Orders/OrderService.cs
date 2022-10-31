using Foodcourt.BusinessLogic.Extensions;
using Foodcourt.Data;
using Foodcourt.Data.Api.Entities.Orders;
using Foodcourt.Data.Api.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Foodcourt.BusinessLogic.Services.Orders;

public class OrderService : IOrderService
{
    private readonly AppDataContext _dataContext;

    public OrderService(AppDataContext dataContext) =>
        _dataContext = dataContext;

    public async Task CreateOrders(string userId)
    {
        var basket = await _dataContext.Baskets.FirstOrDefaultAsync(x => x.AppUserId.Equals(userId));
        if (basket == null)
            throw new Exception("unhandled error when get user basket (user basket is null)");

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
                Comment = "todo",
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
}