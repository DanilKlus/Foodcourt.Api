using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;

namespace Foodcourt.BusinessLogic.Services.Users;

public interface IUserService
{
    Task<UserResponse> GetUserAsync(string userId);
    Task<UserResponse> PatchUserAsync(string userId, PatchUserRequest userRequest);
}