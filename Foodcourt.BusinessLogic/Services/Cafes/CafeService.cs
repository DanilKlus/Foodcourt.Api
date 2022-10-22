using System.Net;
using Foodcourt.BusinessLogic.Extensions;
using Foodcourt.Data;
using Foodcourt.Data.Api;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Foodcourt.Data.Api.Response.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Foodcourt.BusinessLogic.Services.Cafes;

public class CafeService : ICafeService
{
    private readonly AppDataContext _dataContext;
    public CafeService(AppDataContext dataContext) => 
        _dataContext = dataContext;

    public async Task<SearchResponse<CafeResponse>> SearchByQuery(CafeSearchRequest cafeSearch)
    {
        var cafes = await _dataContext.Cafes.ToListAsync();
        return new SearchResponse<CafeResponse>(cafes.ToList().Select(cafe => cafe.ToEntity()).ToList(), cafes.Count);
    }

    public async Task<CafeResponse> GetAsync(long cafeId)
    {
        var cafe = await _dataContext.Cafes.FirstOrDefaultAsync(cafe => Equals(cafe.Id, cafeId));
        if (cafe == null)
            throw new NotFoundException(HttpStatusCode.NotFound, $"Cafe with id '{cafeId}' not found");
        return cafe.ToEntity();
    }

    public async Task<SearchResponse<ProductResponse>> GetProductsAsync(long cafeId)
    {
        var products = await _dataContext.Products.Where(product => Equals(product.CafeId, cafeId)).ToListAsync();
        return new SearchResponse<ProductResponse>(products.Select(product => product.ToEntity()).ToList(), products.Count);
    }

    public async Task<ProductResponse> GetProductAsync(long cafeId, long productId)
    {
        var product = await _dataContext.Products
            .Include(p => p.ProductTypes)
            .FirstOrDefaultAsync(product => product.Id == productId);
        if (product == null) 
            throw new NotFoundException(HttpStatusCode.NotFound, $"Product with id '{productId}' in cafe with id '{cafeId}' not found");
        return product.ToEntity();
    }
}