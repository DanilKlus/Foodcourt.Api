using Foodcourt.Data.Api.Response;

namespace Foodcourt.BusinessLogic.Services.Basket;

public interface IBasketService
{
    Task<BasketResponse> GetBasket(string userId);
}

