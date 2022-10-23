using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Foodcourt.Data.Api.Entities.Users;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Foodcourt.BusinessLogic.Services.Users;

public class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    public UserService(UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }


    public async Task<UserManagerResponse> RegisterUserAsync(UserRegisterRequest userRequest)
    {
        if (userRequest == null)
            throw new NullReferenceException("Request is null");
        var appUser = new AppUser()
        {
            Email = userRequest.Email,
            UserName = userRequest.Email,
            PhoneNumber = userRequest.Phone,
            Name = userRequest.Name
        };
        var result = await _userManager.CreateAsync(appUser, userRequest.Password);
        await _userManager.AddToRoleAsync(appUser, _configuration["AuthSettings:DefaultUserRole"]);
        if (result.Succeeded)
            return new UserManagerResponse
            {
                //TODO: confirm email and create basket 
                Message = "User created successfully",
                IsSuccess = true
            };
        return new UserManagerResponse
        {
            Message = "User did not created",
            IsSuccess = false,
            Errors = result.Errors.Select(e => e.Description).ToList()
        };
    }

    public async Task<UserManagerResponse> LoginUserAsync(UserLoginRequest userRequest)
    {
        var user = await _userManager.FindByEmailAsync(userRequest.Email);
        if (user == null)
            return new UserManagerResponse
            {
                Message = $"User with email '{userRequest.Email}' not found",
                IsSuccess = false
            };
        var result = await _userManager.CheckPasswordAsync(user, userRequest.Password);
        if (!result)
            return new UserManagerResponse
            {
                Message = "Invalid password",
                IsSuccess = false
            };
        
        var claims = new List<Claim>
        {
            new("email", userRequest.Email),
            new("sub", user.Id),
        };
        var userRoles = await _userManager.GetRolesAsync(user);
        foreach (var userRole in userRoles)
            claims.Add(new Claim("roles", userRole));

        var token = GenerateAccessToken(claims);
        var refreshToken = await GenerateRefreshToken(user);
        var tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);
        return new UserLoginResponse
        {
            Message = "User has been successfully authenticated",
            IsSuccess = true,
            AcssessToken = tokenAsString,
            RefreshToken = refreshToken,
            ExpireDate = token.ValidTo
        }; 
    }

    public async Task<UserManagerResponse> RefreshLoginAsync(string refreshToken, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new UserManagerResponse
            {
                Message = $"User with id '{userId}' not found",
                IsSuccess = false
            };
        var result = await _userManager.VerifyUserTokenAsync(user, _configuration["AuthSettings:ApiTokenProvider"], "RefreshToken", refreshToken);
        if (!result)
            return new UserManagerResponse
            {
                Message = "Refresh token not valid",
                IsSuccess = false
            };

        var claims = await _userManager.GetClaimsAsync(user);
        var token = GenerateAccessToken(claims);
        var newRefreshToken = await GenerateRefreshToken(user);
        var tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);
        return new UserLoginResponse
        {
            Message = "Token successfully refreshed",
            IsSuccess = true,
            AcssessToken = tokenAsString,
            RefreshToken = newRefreshToken,
            ExpireDate = token.ValidTo
        }; 
    }

    
    
    
    private SecurityToken GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));
        var token = new JwtSecurityToken(
            issuer: _configuration["AuthSettings:Issuer"],
            audience: _configuration["AuthSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(Convert.ToDouble(_configuration["AuthSettings:TokenLifeDays"])),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return token;
    }

    private async Task<string> GenerateRefreshToken(IdentityUser user)
    {
        //TODO: set expiration date 
        var newRefreshToken = await _userManager.GenerateUserTokenAsync(user, _configuration["AuthSettings:ApiTokenProvider"], "RefreshToken");
        await _userManager.RemoveAuthenticationTokenAsync(user, _configuration["AuthSettings:ApiTokenProvider"], "RefreshToken");
        await _userManager.SetAuthenticationTokenAsync(user, _configuration["AuthSettings:ApiTokenProvider"], "RefreshToken", newRefreshToken);
        return newRefreshToken;
    }
}