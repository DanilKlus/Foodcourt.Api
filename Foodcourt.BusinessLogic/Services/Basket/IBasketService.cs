using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;

namespace Foodcourt.BusinessLogic.Services.Basket;

public interface IBasketService
{
    Task<BasketResponse> GetBasket(string userId);
    Task<long> AddProduct(string userId, AddProductRequest addAddProductRequest);
    Task PatchProduct(string userId, long productId, PatchProductRequest patchProductRequest);
    Task DeleteProduct(string userId, long productId);
    Task CleanBasket(string userId);
}

