using Foodcourt.Data.Api;
using Foodcourt.Data.Api.Entities.Cafes;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;

namespace Foodcourt.BusinessLogic.Services.Cafes;

public interface ICafeService
{
    Task<SearchResponse<CafeResponse>> GetCafesAsync(CafeSearchRequest cafeSearch);
    Task<CafeResponse> GetCafeAsync(long cafeId);
    Task<SearchResponse<ProductResponse>> GetProductsAsync(long cafeId, string? query);
    Task<ProductResponse> GetProductAsync(long cafeId, long productId);
    Task AddCafeAsync(CafeCreateRequest request, string userId);
}