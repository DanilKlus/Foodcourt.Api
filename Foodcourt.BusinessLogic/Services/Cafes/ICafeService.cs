using Foodcourt.Data.Api;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Entities.Response;

namespace Foodcourt.BusinessLogic.Services.Cafes;

public interface ICafeService
{
    Task<SearchResponse<CafeSearchResponse>> SearchByQuery(CafeSearchRequest cafeSearch);
    Task<CafeSearchResponse> Get(Guid cafeId);
}