using Foodcourt.BusinessLogic.Extensions;
using Foodcourt.Data.Api;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Entities.Response;

namespace Foodcourt.BusinessLogic.Services.Cafes;

public class CafeService : ICafeService
{
    private readonly ICafeRepository cafeRepository;

    public CafeService(ICafeRepository cafeRepository)
    {
        this.cafeRepository = cafeRepository;
    }

    public async Task<SearchResponse<CafeSearchResponse>> SearchByQuery(CafeSearchRequest cafeSearch)
    {
        var cafes = await cafeRepository.Search(cafeSearch);
        return new SearchResponse<CafeSearchResponse>(cafes.ToList().Select(cafe => cafe.ToEntity()).ToList(), cafes.Count);
    }

    public async Task<CafeSearchResponse> Get(Guid cafeId)
    {
        var cafe = await cafeRepository.Get(cafeId);
        return cafe.ToEntity();
    }
}