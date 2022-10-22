using Foodcourt.Data;
using Foodcourt.Data.Api.Entities.Users;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Microsoft.AspNetCore.Identity;
using UserRegisterRequest = Foodcourt.Data.Api.Request.UserRegisterRequest;

namespace Foodcourt.BusinessLogic.Services.Cafes;

public class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;
    public UserService(UserManager<IdentityUser> userManager) => 
        _userManager = userManager;


    public async Task<UserRegisterResponse> RegisterUserAsync(UserRegisterRequest userRegisterRequest)
    {
        if (userRegisterRequest == null)
            throw new NullReferenceException("Request is null");
        var identityUser = new AppUser()
        {
            Email = userRegisterRequest.Email,
            UserName = userRegisterRequest.Email,
            PhoneNumber = userRegisterRequest.Phone,
            Name = userRegisterRequest.Name
        };

        var result = await _userManager.CreateAsync(identityUser, userRegisterRequest.Password);
        if (result.Succeeded)
            return new UserRegisterResponse
            {
                //TODO: confirm email and create basket 
                Message = "User created successfully",
                IsSuccess = true
            };
        return new UserRegisterResponse
        {
            Message = "User did not created",
            IsSuccess = false,
            Errors = result.Errors.Select(e => e.Description).ToList()
        };
    }
}