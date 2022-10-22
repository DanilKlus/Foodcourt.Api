using Foodcourt.Data.Api.Response;
using UserRegisterRequest = Foodcourt.Data.Api.Request.UserRegisterRequest;

namespace Foodcourt.BusinessLogic.Services.Cafes;

public interface IUserService
{
   Task<UserRegisterResponse> RegisterUserAsync(UserRegisterRequest userRegisterRequest);
}