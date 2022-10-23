using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;

namespace Foodcourt.BusinessLogic.Services.Users;

public interface IUserService
{
   Task<UserManagerResponse> RegisterUserAsync(UserRegisterRequest userRegisterRequest);
   Task<UserManagerResponse> LoginUserAsync(UserLoginRequest userLoginRequest);
   Task<UserManagerResponse> RefreshLoginAsync(string refreshToken, string userId);
}