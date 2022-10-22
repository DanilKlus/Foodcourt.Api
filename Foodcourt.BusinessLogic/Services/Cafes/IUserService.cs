using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using UserRegisterRequest = Foodcourt.Data.Api.Request.UserRegisterRequest;

namespace Foodcourt.BusinessLogic.Services.Cafes;

public interface IUserService
{
   Task<UserManagerResponse> RegisterUserAsync(UserRegisterRequest userRegisterRequest);
   Task<UserManagerResponse> LoginUserAsync(UserLoginRequest userLoginRequest);
}