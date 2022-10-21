using System.Net;
using Foodcourt.BusinessLogic.Extensions;
using Foodcourt.Data;
using Foodcourt.Data.Api;
using Foodcourt.Data.Api.Entities.Cafes;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Foodcourt.Data.Api.Response.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Foodcourt.BusinessLogic.Services.Cafes;

public class CafeService : ICafeService
{
    private readonly DataContext _dataContext;
    public CafeService(DataContext dataContext) => 
        _dataContext = dataContext;

    public async Task<SearchResponse<CafeSearchResponse>> SearchByQuery(CafeSearchRequest cafeSearch)
    {
        var cafes = await _dataContext.Cafes.ToListAsync();
        return new SearchResponse<CafeSearchResponse>(cafes.ToList().Select(cafe => cafe.ToEntity()).ToList(), cafes.Count);
    }

    public async Task<CafeSearchResponse> Get(long cafeId)
    {
        var cafe = await _dataContext.Cafes.FirstOrDefaultAsync(cafe => Equals(cafe.Id, cafeId));
        if (cafe == null)
            throw new NotFoundException(HttpStatusCode.NotFound, $"Cafe with id = {cafeId} not found");
        return cafe.ToEntity();
    }

    public async Task<SearchResponse<Product>> GetProducts(long cafeId)
    {
        var products = await _dataContext.Products.Where(product => Equals(product.CafeId, cafeId)).ToListAsync();
        return new SearchResponse<Product>(products.ToList().ToList(), products.Count);
    }

    public async Task<Product> GetProduct(long cafeId, long productId)
    {
        var product = await _dataContext.Products.FirstOrDefaultAsync(product => product.Id.Equals(productId));
        if (product == null) 
            throw new NotFoundException(HttpStatusCode.NotFound, $"Product with id = {productId} in cafe with id = {cafeId} not found");
        return product;
    }
}