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
            throw new Exception("unhandled error when get user basket (user basket is null)");
        
        if (basket.Status == BasketStatus.Empty)
            return new BasketResponse {Status = BasketStatus.Empty};

        var products = await _dataContext.BasketProducts
            .Include(x => x.ProductVariant)
            .Include(p => p.Product)
                .ThenInclude(t => t.ProductTypes)
            .Include(p => p.Product)
                .ThenInclude(c => c.Cafe)
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
                Products = products
                    .Where(x => x.Product.CafeId.Equals(cafe.Id))
                    .Select(x => x.ToEntity())
                    .ToList()
            });
        return basketResponse;
    }
    
    public async Task CleanBasket(string userId)
    {
        var basket = await _dataContext.Baskets.FirstOrDefaultAsync(x => x.AppUserId.Equals(userId));
        if (basket == null)
            throw new Exception("unhandled error when get user basket (user basket is null)");
        
        var basketProducts = await _dataContext.BasketProducts.Where(x => x.BasketId.Equals(basket.Id)).ToListAsync();
        basket.Status = BasketStatus.Empty;
        
        _dataContext.BasketProducts.RemoveRange(basketProducts);
        _dataContext.Baskets.Update(basket);
        await _dataContext.SaveChangesAsync();
    }

    public async Task<long> AddProduct(string userId, AddProductRequest addAddProductRequest)
    {
        var basket = await _dataContext.Baskets.FirstOrDefaultAsync(x => x.AppUserId.Equals(userId));
        if (basket == null)
            throw new Exception("unhandled error when get user basket (user basket is null)");
        
        if (basket.Status == BasketStatus.Empty)
        {
            basket.Status = BasketStatus.NotEmpty;
            _dataContext.Baskets.Update(basket);
        }
        
        var basketProduct = new BasketProduct 
        {
            Count = 1, 
            BasketId = basket.Id,
            ProductId = addAddProductRequest.Id, 
            ProductVariantId = addAddProductRequest.VariantId ?? 1L
        };

        var result = _dataContext.BasketProducts.Add(basketProduct);
        await _dataContext.SaveChangesAsync();
        return result.Entity.Id;
    }
    
    public async Task PatchProduct(string userId, long productId, PatchProductRequest patchProductRequest)
    {
        var basket = await _dataContext.Baskets.FirstOrDefaultAsync(x => x.AppUserId.Equals(userId));
        if (basket == null)
            throw new Exception("unhandled error when get user basket (user basket is null)");
        var basketProduct = await _dataContext.BasketProducts.FirstOrDefaultAsync(x => x.BasketId.Equals(basket.Id) &&x.Id.Equals(productId) );
//TODO: add not found

        if (patchProductRequest.VariantId != null)
            basketProduct.ProductVariantId = (long) patchProductRequest.VariantId;
        if (patchProductRequest.Count != null)
            basketProduct.Count = (int) patchProductRequest.Count;

        _dataContext.BasketProducts.Update(basketProduct);
        await _dataContext.SaveChangesAsync();
    }
    
    public async Task DeleteProduct(string userId, long productId)
    {
        var basket = await _dataContext.Baskets.FirstOrDefaultAsync(x => x.AppUserId.Equals(userId));
        if (basket == null)
            throw new Exception("unhandled error when get user basket (user basket is null)");

        var basketProducts = await _dataContext.BasketProducts.Where(x => x.BasketId.Equals(basket.Id)).ToListAsync();
        var basketProduct = basketProducts.FirstOrDefault(x => x.Id.Equals(productId));
        if (basket.Status == BasketStatus.Empty || basketProduct == null)
            return;
        
        if (basketProducts.Count == 1)
        {
            basket.Status = BasketStatus.Empty;
            _dataContext.Baskets.Update(basket);
        }

        _dataContext.BasketProducts.Remove(basketProduct);
        await _dataContext.SaveChangesAsync();
    }
}