using System.Net;
using Foodcourt.BusinessLogic.Extensions;
using Foodcourt.Data;
using Foodcourt.Data.Api.Entities.Users;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Foodcourt.Data.Api.Response.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Foodcourt.BusinessLogic.Services.Basket;

public class BasketService : IBasketService
{
    private readonly AppDataContext _dataContext;
    public BasketService(AppDataContext dataContext) =>
        _dataContext = dataContext;

    public async Task<BasketResponse> GetBasketAsync(string userId)
    {
        var basket = await GetBasketEntityAsync(userId);
        if (basket.Status == BasketStatus.Empty)
            return new BasketResponse {Status = BasketStatus.Empty};

        var basketProducts = await _dataContext.BasketProducts
            .Include(x => x.ProductVariant)
            .Include(p => p.Product)
                .ThenInclude(t => t.ProductTypes)
            .Include(p => p.Product)
                .ThenInclude(c => c.Cafe)
            .Where(product => product.BasketId.Equals(basket.Id))
            .ToListAsync();
        var cafes = basketProducts.DistinctBy(x => x.Product.Cafe.Id).Select(x => x.Product.Cafe).ToList();
        
        var basketResponse = new BasketResponse
        {
            TotalPrice = basketProducts.Select(x => x.Product.Price * x.Count).Sum(),
            TotalProductsCount = basketProducts.Count,
            Status = basket.Status,
            CafesBaskets = new List<CafeBasket>()
        };
        foreach (var cafe in cafes)
            basketResponse.CafesBaskets.Add(new CafeBasket
            {
                Id = cafe.Id,
                Name = cafe.Name,
                Products = basketProducts
                    .Where(x => x.Product.CafeId.Equals(cafe.Id))
                    .Select(x => x.ToEntity())
                    .ToList()
            });
        return basketResponse;
    }
    
    public async Task CleanBasketAsync(string userId)
    {
        var basket = await GetBasketEntityAsync(userId);
        var basketProducts = await _dataContext.BasketProducts
            .Where(x => x.BasketId.Equals(basket.Id)).ToListAsync();
        
        basket.Status = BasketStatus.Empty;
        
        _dataContext.BasketProducts.RemoveRange(basketProducts);
        _dataContext.Baskets.Update(basket);
        await _dataContext.SaveChangesAsync();
    }

    public async Task<long> AddProductAsync(string userId, AddProductRequest addAddProductRequest)
    {
        var basket = await GetBasketEntityAsync(userId);
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
    
    public async Task PatchProductAsync(string userId, long productId, PatchProductRequest patchProductRequest)
    {
        var basket = GetBasketEntityAsync(userId);
        var basketProduct = await _dataContext.BasketProducts
            .FirstOrDefaultAsync(x => x.BasketId.Equals(basket.Id) && x.Id.Equals(productId));
        if (basketProduct == null)
            throw new NotFoundException($"product: {productId} not found");

        if (patchProductRequest.VariantId != null)
            basketProduct.ProductVariantId = (long) patchProductRequest.VariantId;
        if (patchProductRequest.Count != null)
            basketProduct.Count = (int) patchProductRequest.Count;

        _dataContext.BasketProducts.Update(basketProduct);
        await _dataContext.SaveChangesAsync();
    }
    
    public async Task DeleteProductAsync(string userId, long productId)
    {
        var basket = await GetBasketEntityAsync(userId);
        var basketProducts = await _dataContext.BasketProducts
            .Where(x => x.BasketId.Equals(basket.Id)).ToListAsync();
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

    private async Task<Data.Api.Entities.Users.Basket> GetBasketEntityAsync(string userId)
    {
        var basket = await _dataContext.Baskets.FirstOrDefaultAsync(x => x.AppUserId.Equals(userId));
        if (basket == null)
            throw new Exception($"unhandled error when get user: {userId} basket (user basket is null)");
        return basket;
    }
}