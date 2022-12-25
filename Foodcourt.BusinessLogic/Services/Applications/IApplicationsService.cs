using Foodcourt.Data.Api;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;

namespace Foodcourt.BusinessLogic.Services.Applications;

public interface IApplicationsService
{
    Task<SearchResponse<CafeApplicationResponse>> GetMyCafesApplicationsAsync(string userId);
    
    Task<FullApplicationEntity> GetMyCafeApplicationAsync(string userId, long cafeId);
    Task<SearchResponse<CafeApplicationResponse>> GetCafesApplicationsAsync(SearchApplicationRequest request);
    Task SetCafeStatusAsync(long cafeId, bool status, string response);
}