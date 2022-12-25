using Foodcourt.BusinessLogic.Extensions;
using Foodcourt.Data;
using Foodcourt.Data.Api;
using Foodcourt.Data.Api.Entities.Cafes;
using Foodcourt.Data.Api.Entities.Users;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Foodcourt.Data.Api.Response.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Foodcourt.BusinessLogic.Services.Applications;

public class ApplicationsService : IApplicationsService
{
    private readonly AppDataContext _dataContext;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    public ApplicationsService(AppDataContext dataContext, UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _dataContext = dataContext;
        _userManager = userManager;
        _configuration = configuration;
    }


    public async Task<SearchResponse<CafeApplicationResponse>> GetMyCafesApplicationsAsync(string userId)
    {
        var cafes = await _dataContext.Cafes.Include(x => x.AppUsers)
            .Where(x => x.AppUsers.Select(cafeUser => cafeUser.Id).Contains(userId)).ToListAsync();
        
        return new SearchResponse<CafeApplicationResponse>(cafes.Select(x => x.ToApplicationEntity()).ToList(), cafes.Count);
    }

    public async Task<CafeApplicationResponse> GetMyCafeApplicationAsync(string userId, long cafeId)
    {
        var cafe = await _dataContext.Cafes.Include(x => x.AppUsers)
            .FirstOrDefaultAsync(x => x.AppUsers.Select(cafeUser => cafeUser.Id).Contains(userId));
        if (cafe == null)
            throw new NotFoundException($"application with id '{cafeId}' not found");
        
        return cafe.ToApplicationEntity();
    }

    public async Task<SearchResponse<CafeApplicationResponse>> GetCafesApplicationsAsync(SearchApplicationRequest request)
    {
        var skipCount = request.Skip ?? 0;
        var takeCount = request.Take ?? 50;
        var query = request.Query ?? "";
        
        var rawCafes = await _dataContext.Cafes.Where(cafe => cafe.Name.ToLower().Contains(query.ToLower())).ToListAsync();
        var cafes = rawCafes;
        if (request.Status != null)
            cafes = rawCafes.Where(cafe => cafe.Status.Equals(request.Status)).ToList();
        return new SearchResponse<CafeApplicationResponse>(cafes.Skip(skipCount).Take(takeCount).Select(x => x.ToApplicationEntity()).ToList(), cafes.Count);
    }

    public async Task SetCafeStatusAsync(long cafeId, bool status, string response)
    {
        var cafe = await _dataContext.Cafes.Include(x => x.AppUsers).FirstOrDefaultAsync(cafe => Equals(cafe.Id, cafeId));
        var user = cafe.AppUsers.First();
        if (cafe == null)
            throw new NotFoundException($"Cafe with id '{cafeId}' not found");

        cafe.Status = status ? CafeStatus.Approved : CafeStatus.Rejected;
        cafe.IsActive = status;
        cafe.Response = response;
    
        _dataContext.Cafes.Update(cafe);
        await _dataContext.SaveChangesAsync();
        if (status)
            await _userManager.AddToRoleAsync(user, CustomRoles.Director);
    }
}