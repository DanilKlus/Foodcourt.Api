using Foodcourt.Data.Api;
using Foodcourt.Data.Api.Entities.Cafes;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;

namespace Foodcourt.BusinessLogic.Services.Cafes;

public interface ICafeService
{
    Task<SearchResponse<CafeResponse>> SearchByQuery(CafeSearchRequest cafeSearch);
    Task<CafeResponse> GetAsync(long cafeId);
    Task<SearchResponse<ProductResponse>> GetProductsAsync(long cafeId);
    Task<ProductResponse> GetProductAsync(long cafeId, long productId);
}