using Foodcourt.Data.Api.Entities.Users;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Microsoft.AspNetCore.Identity;

namespace Foodcourt.BusinessLogic.Services.Users;

public interface IUserService
{
   Task<UserManagerResponse> RegisterUserAsync(UserRegisterRequest userRegisterRequest);
   Task<UserManagerResponse> LoginUserAsync(UserLoginRequest userLoginRequest);
   Task<UserManagerResponse> RefreshLoginAsync(string refreshToken, string userId);
   Task<UserManagerResponse> ExternalLogin(ExternalLoginInfo info);
}