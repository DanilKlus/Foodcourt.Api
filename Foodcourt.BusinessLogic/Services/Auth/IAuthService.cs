using Foodcourt.Data.Api.Entities.Users;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Microsoft.AspNetCore.Identity;

namespace Foodcourt.BusinessLogic.Services.Users;

public interface IAuthService
{
   Task<AuthManagerResponse> RegisterUserAsync(UserRegisterRequest userRegisterRequest);
   Task<AuthManagerResponse> LoginUserAsync(UserLoginRequest userLoginRequest);
   Task<AuthManagerResponse> RefreshLoginAsync(string refreshToken, string userId);
   Task<AuthManagerResponse> ExternalLogin(ExternalLoginInfo info);
}