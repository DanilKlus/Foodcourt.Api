using Foodcourt.BusinessLogic.Extensions;
using Foodcourt.Data;
using Foodcourt.Data.Api;
using Foodcourt.Data.Api.Entities.Cafes;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Foodcourt.Data.Api.Response.Exceptions;
using GeoCoordinatePortable;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Foodcourt.BusinessLogic.Services.Cafes;

public class CafeService : ICafeService
{
    private readonly AppDataContext _dataContext;
    private readonly UserManager<IdentityUser> _userManager;
    public CafeService(AppDataContext dataContext, UserManager<IdentityUser> userManager)
    {
        _dataContext = dataContext;
        _userManager = userManager;
    }

    public async Task<SearchResponse<CafeResponse>> GetCafesAsync(CafeSearchRequest cafeSearch)
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

    public async Task<CafeResponse> GetCafeAsync(long cafeId)
    {
        var cafe = await _dataContext.Cafes.FirstOrDefaultAsync(cafe => cafe.IsActive && Equals(cafe.Id, cafeId));
        if (cafe == null)
            throw new NotFoundException($"Cafe with id '{cafeId}' not found");
        return cafe.ToEntity();
    }

    public async Task<SearchResponse<ProductResponse>> GetProductsAsync(long? cafeId, SearchRequest searchRequest)
    {
        var skipCount = searchRequest.Skip ?? 0;
        var takeCount = searchRequest.Take ?? 50;
        
        List<Product> products;
        if (cafeId != null)
            products = await _dataContext.Products.Where(product => Equals(product.CafeId, cafeId) && product.Name.Contains(searchRequest.Query ?? "")).ToListAsync();
        else
            products = await _dataContext.Products.Where(product => product.Name.Contains(searchRequest.Query ?? "")).ToListAsync();
        
        var filteredProducts = products;
        if (!string.IsNullOrEmpty(searchRequest.Query))
            filteredProducts = products.Where(product => product.Name.ToLower().Contains(searchRequest.Query.ToLower())).
                Skip(skipCount).Take(takeCount).ToList();
        return new SearchResponse<ProductResponse>(filteredProducts.Select(product => product.ToEntity()).ToList(), filteredProducts.Count);
    }

    public async Task<ProductResponse> GetProductAsync(long cafeId, long productId)
    {
        var product = await _dataContext.Products
            .Include(p => p.ProductVariants)
            .Include(p => p.ProductTypes)
            .FirstOrDefaultAsync(product => product.Id == productId);
        if (product == null) 
            throw new NotFoundException($"Product with id '{productId}' in cafe with id '{cafeId}' not found");
        return product.ToEntity();
    }

    public async Task AddCafeAsync(CafeCreateRequest cafeRequest, string userId)
    {
        var user = await _dataContext.AppUsers.FirstAsync(x => x.Id.Equals(userId));
        user.Cafes = new List<Cafe> { cafeRequest.FromEntity() };

        await _dataContext.SaveChangesAsync();
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