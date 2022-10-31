using System.Net;
using Foodcourt.BusinessLogic.Extensions;
using Foodcourt.Data;
using Foodcourt.Data.Api;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Foodcourt.Data.Api.Response.Exceptions;
using GeoCoordinatePortable;
using Microsoft.EntityFrameworkCore;

namespace Foodcourt.BusinessLogic.Services.Cafes;

public class CafeService : ICafeService
{
    private readonly AppDataContext _dataContext;
    public CafeService(AppDataContext dataContext) => 
        _dataContext = dataContext;

    public async Task<SearchResponse<CafeResponse>> SearchByQuery(CafeSearchRequest cafeSearch)
    {
        var skipCount = cafeSearch.Skip ?? 0;
        var takeCount = cafeSearch.Take ?? 50;
        var cafesEntities = await _dataContext.Cafes.ToListAsync();
        var cafes = cafesEntities.
            OrderBy(x => GetDistance(x.Latitude, x.Longitude, cafeSearch.Latitude, cafeSearch.Longitude))
            .Where(x => x.IsActive && x.Name.Contains(cafeSearch.Name ?? ""))
            .Skip(skipCount).Take(takeCount)
            .ToList();
            
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
            .Include(p => p.ProductVariants)
            .Include(p => p.ProductTypes)
            .FirstOrDefaultAsync(product => product.Id == productId);
        if (product == null) 
            throw new NotFoundException(HttpStatusCode.NotFound, $"Product with id '{productId}' in cafe with id '{cafeId}' not found");
        return product.ToEntity();
    }
    
    private static double GetDistance(double cafeLatitude, double cafeLongitude, double? userLatitude, double? userLongitude)
    {
        if (userLatitude == null || userLongitude == null) 
            return 1;
        var userCoord = new GeoCoordinate((double)userLatitude, (double)userLongitude);
        var cafeCoord = new GeoCoordinate(cafeLatitude, cafeLongitude);
        return userCoord.GetDistanceTo(cafeCoord);
    }
}