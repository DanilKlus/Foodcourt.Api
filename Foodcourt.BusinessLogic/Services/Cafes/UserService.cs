using Foodcourt.Data;
using Foodcourt.Data.Api.Entities.Users;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Microsoft.AspNetCore.Identity;

namespace Foodcourt.BusinessLogic.Services.Cafes;

public class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;
    public UserService(UserManager<IdentityUser> userManager) => 
        _userManager = userManager;


    public async Task<UserCreateResponse> RegisterUserAsync(CreateUserRequest userRequest)
    {
        if (userRequest == null)
            throw new NullReferenceException("Request is null");
        var identityUser = new AppUser()
        {
            Email = userRequest.Email,
            UserName = userRequest.Email,
            PhoneNumber = userRequest.Phone,
            Name = userRequest.Name
        };

        var result = await _userManager.CreateAsync(identityUser, userRequest.Password);
        if (result.Succeeded)
            return new UserCreateResponse
            {
                //TODO: confirm email and create basket 
                Message = "User created successfully",
                IsSuccess = true
            };
        return new UserCreateResponse
        {
            Message = "User did not created",
            IsSuccess = false,
            Errors = result.Errors.Select(e => e.Description).ToList()
        };
    }
}