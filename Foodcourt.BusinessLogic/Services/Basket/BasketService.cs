using Foodcourt.BusinessLogic.Extensions;
using Foodcourt.Data;
using Foodcourt.Data.Api.Entities.Users;
using Foodcourt.Data.Api.Response;
using Microsoft.EntityFrameworkCore;

namespace Foodcourt.BusinessLogic.Services.Basket;

public class BasketService : IBasketService
{
    private readonly AppDataContext _dataContext;

    public BasketService(AppDataContext dataContext) =>
        _dataContext = dataContext;

    public async Task<BasketResponse> GetBasket(string userId)
    {
        var basket = await _dataContext.Baskets.FirstOrDefaultAsync(basket => basket.AppUserId.Equals(userId));
        if (basket == null)
            throw new Exception("unhandled error when get user basket");
        if (basket.Status == BasketStatus.Empty)
            return new BasketResponse {Status = BasketStatus.Empty};

        var products = await _dataContext.BasketProducts
            .Include(x => x.ProductVariant)
            .Include(p => p.Product).ThenInclude(t => t.ProductTypes)
            .Include(p => p.Product).ThenInclude(c => c.Cafe)
            .Where(product => product.BasketId.Equals(basket.Id))
            .ToListAsync();

        var basketResponse = new BasketResponse
        {
            TotalPrice = products.Select(x => x.Product.Price).Sum(),
            TotalProductsCount = products.Count,
            Status = basket.Status,
            CafesBaskets = new List<CafeBasket>()
        };

        var cafes = products.DistinctBy(x => x.Product.Cafe.Id).Select(x => x.Product.Cafe).ToList();
        foreach (var cafe in cafes)
            basketResponse.CafesBaskets.Add(new CafeBasket
            {
                Id = cafe.Id,
                Name = cafe.Name,
                Products = products.Where(x => x.Product.CafeId.Equals(cafe.Id)).Select(x => x.ToEntity()).ToList()
            });

        return basketResponse;
    }
}