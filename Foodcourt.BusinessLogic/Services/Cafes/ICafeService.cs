using Foodcourt.Data.Api;
using Foodcourt.Data.Api.Entities.Cafes;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;

namespace Foodcourt.BusinessLogic.Services.Cafes;

public interface ICafeService
{
    Task<SearchResponse<CafeSearchResponse>> SearchByQuery(CafeSearchRequest cafeSearch);
    Task<CafeSearchResponse> Get(long cafeId);
    Task<SearchResponse<Product>> GetProducts(long cafeId);
    Task<Product> GetProduct(long cafeId, long productId);
}