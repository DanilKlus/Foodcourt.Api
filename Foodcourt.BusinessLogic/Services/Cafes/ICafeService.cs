using Foodcourt.Data.Api;
using Foodcourt.Data.Api.Entities.Cafes;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;

namespace Foodcourt.BusinessLogic.Services.Cafes;

public interface ICafeService
{
    Task<SearchResponse<CafeResponse>> SearchByQuery(CafeSearchRequest cafeSearch);
    Task<CafeResponse> Get(long cafeId);
    Task<SearchResponse<ProductResponse>> GetProducts(long cafeId);
    Task<ProductResponse> GetProduct(long cafeId, long productId);
}