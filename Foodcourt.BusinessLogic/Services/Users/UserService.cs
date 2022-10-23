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
    private readonly SignInManager<IdentityUser> _signInManager;

    public UserService(UserManager<IdentityUser> userManager, IConfiguration configuration,
        SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _configuration = configuration;
        _signInManager = signInManager;
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

        var claims = new List<Claim>();
        var token = await GenerateAccessToken(user, claims);
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
        var result = await _userManager.VerifyUserTokenAsync(user, _configuration["AuthSettings:ApiTokenProvider"],
            "RefreshToken", refreshToken);
        if (!result)
            return new UserManagerResponse
            {
                Message = "Refresh token not valid",
                IsSuccess = false
            };

        var claims = await _userManager.GetClaimsAsync(user);
        var token = await GenerateAccessToken(user, claims);
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


    private async Task<JwtSecurityToken> GenerateAccessToken(IdentityUser user, ICollection<Claim> claims)
    {
        claims.Add(new("email", user.Email));
        claims.Add(new("sub", user.Id));
        var userRoles = await _userManager.GetRolesAsync(user);
        foreach (var userRole in userRoles)
            claims.Add(new Claim("roles", userRole));
        
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
        var newRefreshToken =
            await _userManager.GenerateUserTokenAsync(user, _configuration["AuthSettings:ApiTokenProvider"],
                "RefreshToken");
        await _userManager.RemoveAuthenticationTokenAsync(user, _configuration["AuthSettings:ApiTokenProvider"],
            "RefreshToken");
        await _userManager.SetAuthenticationTokenAsync(user, _configuration["AuthSettings:ApiTokenProvider"],
            "RefreshToken", newRefreshToken);
        return newRefreshToken;
    }


    public async Task<UserManagerResponse> ExternalLogin(ExternalLoginInfo info)
    {
        var signinResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true);
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var user = await _userManager.FindByEmailAsync(email);
        var claims = user != null ? await _userManager.GetClaimsAsync(user) : new List<Claim>();

        if (signinResult.Succeeded && user != null)
        {
            var token = await GenerateAccessToken(user, claims);
            var refreshToken = await GenerateRefreshToken(user);
            await _userManager.SetAuthenticationTokenAsync(user, TokenOptions.DefaultProvider, "RefreshToken", refreshToken);

            return new UserLoginResponse
            {
                AcssessToken = new JwtSecurityTokenHandler().WriteToken(token),
                ExpireDate = token.ValidTo,
                RefreshToken = refreshToken,
                Message = "User successfully SignIn from the external system",
                IsSuccess = true
            };
        }

        if (email != null)
        {
            if (user == null)
            {
                user = new AppUser()
                {
                    UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                    Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                    PhoneNumber = info.Principal.FindFirstValue(ClaimTypes.MobilePhone),
                    Name = info.Principal.FindFirstValue(ClaimTypes.Name),
                    
                };
                await _userManager.CreateAsync(user);
            }
        
            await _userManager.AddLoginAsync(user, info);
            await _signInManager.SignInAsync(user, false);
            
            var token = await GenerateAccessToken(user, claims);
            var refreshToken = await GenerateRefreshToken(user);
            await _userManager.SetAuthenticationTokenAsync(user, TokenOptions.DefaultEmailProvider, "RefreshToken", refreshToken);
        
        
            return new UserLoginResponse
            {
                AcssessToken = new JwtSecurityTokenHandler().WriteToken(token),
                ExpireDate = token.ValidTo,
                RefreshToken = refreshToken,
                Message = "User successfully Sign In from the external system",
                IsSuccess = true
            };
        }

        return new UserManagerResponse()
        {
            Message = "Error when trying to create a user from an external system",
            IsSuccess = false
        };
    }
}