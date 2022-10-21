using Foodcourt.BusinessLogic.Extensions;
using Foodcourt.Data;
using Foodcourt.Data.Api;
using Foodcourt.Data.Api.Entities.Cafes;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Microsoft.EntityFrameworkCore;

namespace Foodcourt.BusinessLogic.Services.Cafes;

public class CafeService : ICafeService
{
    private readonly DataContext dataContext;
    public CafeService(DataContext dataContext) => 
        this.dataContext = dataContext;

    public async Task<SearchResponse<CafeSearchResponse>> SearchByQuery(CafeSearchRequest cafeSearch)
    {
        var cafes = await dataContext.Cafes.ToListAsync();
        return new SearchResponse<CafeSearchResponse>(cafes.ToList().Select(cafe => cafe.ToEntity()).ToList(), cafes.Count);
    }

    public async Task<CafeSearchResponse> Get(long cafeId)
    {
        //TODO add notfound
        var cafe = await dataContext.Cafes.FirstOrDefaultAsync(cafe => Equals(cafe.Id, cafeId));
        if (cafe != null) return cafe.ToEntity();
        throw new Exception();
    }

    public async Task<SearchResponse<Product>> GetProducts(long cafeId)
    {
        var products = await dataContext.Products.Where(product => Equals(product.CafeId, cafeId)).ToListAsync();
        return new SearchResponse<Product>(products.ToList().ToList(), products.Count);
    }
}