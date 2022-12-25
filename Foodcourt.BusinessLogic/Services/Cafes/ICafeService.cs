using Foodcourt.Data.Api;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;

namespace Foodcourt.BusinessLogic.Services.Cafes;

public interface ICafeService
{
    Task<SearchResponse<CafeResponse>> GetCafesAsync(CafeSearchRequest cafeSearch);
    Task<CafeResponse> GetCafeAsync(long cafeId);
    Task<SearchResponse<ProductResponse>> GetProductsAsync(long cafeId, SearchRequest searchRequest);
    Task<ProductResponse> GetProductAsync(long cafeId, long productId);
    Task AddCafeAsync(CafeCreateRequest request, string userId);
    Task PatchCafeAsync(PatchCafeRequest request, string userId, long cafeId);
    Task DeleteCafeAsync(string userId, long cafeId);
    Task<List<SearchResponse>> SearchAsync(CafeSearchRequest request);
    Task DeleteCafeProductAsync(string userId, long cafeId, long productId);
    Task CreateCafeProductAsync(CreateProductRequest request, long cafeId, string userId);
    Task PatchCafeProductAsync(UpdateProductRequest request, long cafeId, long productId, string userId);
}