using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;

namespace Foodcourt.BusinessLogic.Services.Cafes;

public interface IUserService
{
   Task<UserCreateResponse> RegisterUserAsync(CreateUserRequest userRequest);
}