using Foodcourt.BusinessLogic.Extensions;
using Foodcourt.Data;
using Foodcourt.Data.Api.Entities.Users;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Microsoft.EntityFrameworkCore;

namespace Foodcourt.BusinessLogic.Services.Basket;

public class BasketService : IBasketService
{
    //TODO: refactor and add tests
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
        var cafes = products.DistinctBy(x => x.Product.Cafe.Id).Select(x => x.Product.Cafe).ToList();
        
        var basketResponse = new BasketResponse
        {
            TotalPrice = products.Select(x => x.Product.Price).Sum(),
            TotalProductsCount = products.Count,
            Status = basket.Status,
            CafesBaskets = new List<CafeBasket>()
        };
        foreach (var cafe in cafes)
            basketResponse.CafesBaskets.Add(new CafeBasket
            {
                Id = cafe.Id,
                Name = cafe.Name,
                Products = products.Where(x => x.Product.CafeId.Equals(cafe.Id)).Select(x => x.ToEntity()).ToList()
            });
        return basketResponse;
    }
    
    public async Task CleanBasket(string userId)
    {
        var basket = await _dataContext.Baskets.FirstOrDefaultAsync(x => x.AppUserId.Equals(userId));
        if (basket == null)
            throw new Exception("unhandled error when get user basket");
        
        var basketProducts = await _dataContext.BasketProducts.Where(x => x.BasketId.Equals(basket.Id)).ToListAsync();
        _dataContext.BasketProducts.RemoveRange(basketProducts);
        await _dataContext.SaveChangesAsync();
    }

    public async Task<long> AddProduct(string userId, AddProductRequest addAddProductRequest)
    {
        var basket = await _dataContext.Baskets.FirstOrDefaultAsync(x => x.AppUserId.Equals(userId));
        if (basket == null)
            throw new Exception("unhandled error when get user basket");
        if (basket.Status == BasketStatus.Empty)
        {
            basket.Status = BasketStatus.NotEmpty;
            _dataContext.Baskets.Update(basket);
            await _dataContext.SaveChangesAsync();
        }
        
        var basketProduct = new BasketProduct {Count = 1, ProductId = addAddProductRequest.Id, BasketId = basket.Id};
        if (addAddProductRequest.VariantId != null)
            basketProduct.ProductVariantId = (long) addAddProductRequest.VariantId;

        var result = _dataContext.BasketProducts.Add(basketProduct);
        await _dataContext.SaveChangesAsync();
        return result.Entity.Id;
    }
    
    public async Task PatchProduct(string userId, long productId, PatchProductRequest patchProductRequest)
    {
        var basketProduct = await _dataContext.BasketProducts.FirstOrDefaultAsync(x => x.Id.Equals(productId));
        if (basketProduct == null)
            throw new Exception("not found");
        
        if (patchProductRequest.VariantId != null)
            basketProduct.ProductVariantId = (long) patchProductRequest.VariantId;
        if (patchProductRequest.Count != null)
            basketProduct.Count = (int) patchProductRequest.Count;

        _dataContext.BasketProducts.Update(basketProduct);
        await _dataContext.SaveChangesAsync();
    }
    
    public async Task DeleteProduct(string userId, long productId)
    {
        var basketProduct = await _dataContext.BasketProducts.FirstOrDefaultAsync(x => x.Id.Equals(productId));
        if (basketProduct == null)
            return;
        
        _dataContext.BasketProducts.Remove(basketProduct);
        await _dataContext.SaveChangesAsync();
    }
}