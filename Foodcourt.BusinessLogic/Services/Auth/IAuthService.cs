using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Microsoft.AspNetCore.Identity;

namespace Foodcourt.BusinessLogic.Services.Auth;

public interface IAuthService
{
   Task<AuthManagerResponse> RegisterUserAsync(UserRegisterRequest userRegisterRequest);
   Task<AuthManagerResponse> LoginUserAsync(UserLoginRequest userLoginRequest);
   Task<AuthManagerResponse> RefreshTokenAsync(string refreshToken, string userId);
   Task<AuthManagerResponse> ExternalLoginAsync(ExternalLoginInfo info);
   Task SendConfirmationCode(string email);
   Task ConfirmCode(string email, int code);
}