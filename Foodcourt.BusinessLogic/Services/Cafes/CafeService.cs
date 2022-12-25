using Foodcourt.BusinessLogic.Extensions;
using Foodcourt.Data;
using Foodcourt.Data.Api;
using Foodcourt.Data.Api.Entities.Cafes;
using Foodcourt.Data.Api.Entities.Users;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Foodcourt.Data.Api.Response.Exceptions;
using GeoCoordinatePortable;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Foodcourt.BusinessLogic.Services.Cafes;

public class CafeService : ICafeService
{
    private readonly AppDataContext _dataContext;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    public CafeService(AppDataContext dataContext, UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _dataContext = dataContext;
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<SearchResponse<CafeResponse>> GetCafesAsync(CafeSearchRequest cafeSearch)
    {
        var skipCount = cafeSearch.Skip ?? 0;
        var takeCount = cafeSearch.Take ?? 50;
        var cafesEntities = await _dataContext.Cafes.ToListAsync();
        var cafeToDistance = new Dictionary<long, double>();
        var cafes = cafesEntities.
            OrderBy(x =>
            {
                var distance = GetDistance(x.Latitude, x.Longitude, cafeSearch.Latitude, cafeSearch.Longitude);
                cafeToDistance[x.Id] = distance;
                return distance;

            })
            .Where(x => x.IsActive && x.Name.Contains(cafeSearch.Query ?? ""))
            .ToList();
            
        return new SearchResponse<CafeResponse>(cafes.Skip(skipCount).Take(takeCount).ToList().Select(cafe => cafe.ToEntity(cafeToDistance[cafe.Id])).ToList(), cafes.Count);
    }

    public async Task<CafeResponse> GetCafeAsync(long cafeId)
    {
        var cafe = await _dataContext.Cafes.FirstOrDefaultAsync(cafe => cafe.IsActive && Equals(cafe.Id, cafeId));
        if (cafe == null)
            throw new NotFoundException($"Cafe with id '{cafeId}' not found");
        return cafe.ToEntity();
    }

    public async Task<SearchResponse<ProductResponse>> GetProductsAsync(long cafeId, SearchRequest searchRequest)
    {
        var skipCount = searchRequest.Skip ?? 0;
        var takeCount = searchRequest.Take ?? 50;
        var query = searchRequest.Query ?? "";

        var products = await _dataContext.Products
            .Where(product => Equals(product.CafeId, cafeId) && product.Name.ToLower().Contains(query.ToLower())).ToListAsync();
        
        return new SearchResponse<ProductResponse>(products.
            Skip(skipCount).Take(takeCount).Select(product => product.ToEntity()).ToList(), products.Count);
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
        
        await _userManager.AddToRoleAsync(user, CustomRoles.Director);
        await _dataContext.SaveChangesAsync();
    }

    public async Task SetCafeStatusAsync(long cafeId, bool status, string responce)
    {
        var cafe = await _dataContext.Cafes.Include(x => x.AppUsers).FirstOrDefaultAsync(cafe => Equals(cafe.Id, cafeId));
        if (cafe == null)
            throw new NotFoundException($"Cafe with id '{cafeId}' not found");
        
        cafe.IsActive = status;
        cafe.Approved = status;
        cafe.Response = responce;

        _dataContext.Cafes.Update(cafe);
        await _dataContext.SaveChangesAsync();
    }

    public async Task PatchCafeAsync(PatchCafeRequest request, string userId, long cafeId)
    {
        await CheckAccess(userId, cafeId);
        var cafe = await _dataContext.Cafes.Include(x => x.AppUsers).FirstOrDefaultAsync(cafe => Equals(cafe.Id, cafeId));

        if (request.Name != null)
            cafe.Name = request.Name;
        if (request.Description != null)
            cafe.Description = request.Description;
        if (request.PersonalAccount != null)
            cafe.PersonalAccount = request.PersonalAccount;

        _dataContext.Cafes.Update(cafe);
        await _dataContext.SaveChangesAsync();
    }

    public async Task DeleteCafeAsync(string userId, long cafeId)
    {
        await CheckAccess(userId, cafeId);
        var cafe = await _dataContext.Cafes.Include(x => x.AppUsers).FirstOrDefaultAsync(cafe => Equals(cafe.Id, cafeId));

        cafe.IsActive = false;

        _dataContext.Cafes.Update(cafe);
        await _dataContext.SaveChangesAsync();
    }

    public async Task<List<SearchResponse>> SearchAsync(CafeSearchRequest request)
    {
        var skipCount = request.Skip ?? 0;
        var takeCount = request.Take ?? 50;
        var query = request.Query ?? "";
        
        var products = await _dataContext.Products.Where(product => product.Name.ToLower().Contains(query.ToLower())).ToListAsync();
        var cafes = await _dataContext.Cafes.Where(cafe => cafe.IsActive && cafe.Name.ToLower().Contains(query.ToLower())).ToListAsync();

        var cafesIds = cafes.Select(cafe => cafe.Id);
        foreach (var product in products)
        {
            if (!cafesIds.Contains(product.CafeId))
            {
                var cafe = await _dataContext.Cafes.Where(cafe => cafe.IsActive && cafe.Id == product.CafeId).FirstOrDefaultAsync();
                if (cafe == null) continue;
                cafes.Add(cafe);
            }
        }

        return cafes.Select(cafe => cafe.ToSearchResponse(
            GetDistance(cafe.Latitude, cafe.Longitude, request.Latitude, request.Longitude), 
            products.Where(product => product.CafeId == cafe.Id).ToList())).Skip(skipCount).Take(takeCount).ToList();
    }

    public async Task<SearchResponse<CafeApplicationResponse>> GetCafesApplicationsAsync(SearchApplicationRequest request, bool isAdmin, string userId)
    {
        var skipCount = request.Skip ?? 0;
        var takeCount = request.Take ?? 50;
        var query = request.Query ?? "";
        
        var cafes = await _dataContext.Cafes.Include(x => x.AppUsers).Where(cafe => 
            cafe.Approved == request.Approved &&
            cafe.IsActive == request.IsActive 
            && cafe.Name.ToLower().Contains(query.ToLower()))
            .ToListAsync();
        
        if (isAdmin)
            return new SearchResponse<CafeApplicationResponse>(cafes.Skip(skipCount).Take(takeCount).Select(x => x.ToApplicationEntity()).ToList(), cafes.Count);

        var applications = cafes.Skip(skipCount).Take(takeCount)
            .Where(x => x.AppUsers.Select(cafeUser => cafeUser.Id).Contains(userId)).ToList();
        return new SearchResponse<CafeApplicationResponse>(applications.Select(x => x.ToApplicationEntity()).ToList(), applications.Count);
    }

    public async Task DeleteCafeProductAsync(string userId, long cafeId, long productId)
    {
        await CheckAccess(userId, cafeId);

        var product = await _dataContext.Products.FirstOrDefaultAsync(x => x.CafeId.Equals(cafeId) && x.Id.Equals(productId));
        if (product != null)
        {
            _dataContext.Products.Remove(product);
            await _dataContext.SaveChangesAsync();
        }
    }

    public async Task CreateCafeProductAsync(CreateProductRequest request, long cafeId, string userId)
    {
        await CheckAccess(userId, cafeId);
        var cafe = await _dataContext.Cafes.FirstOrDefaultAsync(cafe => Equals(cafe.Id, cafeId));
        var product = request.ToDbEntity(cafe);

        _dataContext.Products.Add(product);
        await _dataContext.SaveChangesAsync();
    }

    private async Task CheckAccess(string userId, long cafeId)
    {
        var cafe = await _dataContext.Cafes.Include(x => x.AppUsers).FirstOrDefaultAsync(cafe => Equals(cafe.Id, cafeId));
        if (cafe == null)
            throw new NotFoundException($"Cafe with id '{cafeId}' not found");
        if (!cafe.AppUsers.Select(x => x.Id).Contains(userId))
            throw new NotHaveAccessException("the user does not have access to the cafe");
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