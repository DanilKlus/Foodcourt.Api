using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;

namespace Foodcourt.BusinessLogic.Services.Basket;

public interface IBasketService
{
    Task<BasketResponse> GetBasketAsync(string userId);
    Task<long> AddProductAsync(string userId, AddProductRequest addAddProductRequest);
    Task PatchProductAsync(string userId, long productId, PatchProductRequest patchProductRequest);
    Task DeleteProductAsync(string userId, long productId);
    Task CleanBasketAsync(string userId);
}

