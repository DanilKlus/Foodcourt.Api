using Foodcourt.Data;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Foodcourt.BusinessLogic.Services.Users;

public class UserService : IUserService
{
    private readonly AppDataContext _dataContext;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    public UserService(AppDataContext dataContext, UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _dataContext = dataContext;
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<UserResponse> GetUserAsync(string userId)
    {
        var user = await _dataContext.AppUsers.FirstAsync(x => x.Id.Equals(userId));
        return new UserResponse
        {
            Name = user.Name,
            Email = user.Email,
            Phone = user.PhoneNumber
        };
    }

    public async Task<UserResponse> PatchUserAsync(string userId, PatchUserRequest userRequest)
    {
        var user = await _dataContext.AppUsers.FirstAsync(x => x.Id.Equals(userId));

        if (!string.IsNullOrEmpty(userRequest.Name))
            user.Name = userRequest.Name;
        if (!string.IsNullOrEmpty(userRequest.Email))
            user.Email = userRequest.Email;
        if (!string.IsNullOrEmpty(userRequest.Phone))
            user.PhoneNumber = userRequest.Phone;
        await _userManager.UpdateAsync(user);
        
        if (!string.IsNullOrEmpty(userRequest.OldPassword) && !string.IsNullOrEmpty(userRequest.NewPassword))
        {
            var changePassword = await _userManager.ChangePasswordAsync(user, userRequest.OldPassword, userRequest.NewPassword);
            if (changePassword.Succeeded)
                await _userManager.RemoveAuthenticationTokenAsync(user, _configuration["AuthSettings:ApiTokenProvider"],
                    "RefreshToken");
            else
                throw new Exception("incorrect password"); //TOD: send normal exception
        }

        return await GetUserAsync(userId);
    }
}