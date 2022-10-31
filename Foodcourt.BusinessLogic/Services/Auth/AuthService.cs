using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Foodcourt.Data;
using Foodcourt.Data.Api.Entities.Users;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Foodcourt.BusinessLogic.Services.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AuthService(UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _configuration = configuration;
        _signInManager = signInManager;
    }


    public async Task<AuthManagerResponse> RegisterUserAsync(UserRegisterRequest userRequest)
    {
        //TODO create google users and throw email and pass errors
        var appUser = new AppUser()
        {
            Email = userRequest.Email,
            UserName = userRequest.Email,
            PhoneNumber = userRequest.Phone,
            Name = userRequest.Name,
            Basket = new Data.Api.Entities.Users.Basket(),
        };
        var createUserResult = await _userManager.CreateAsync(appUser, userRequest.Password);
        var addRoleResult = await _userManager.AddToRoleAsync(appUser, _configuration["AuthSettings:DefaultUserRole"]);
        if (createUserResult.Succeeded && addRoleResult.Succeeded)
            return new AuthManagerResponse
            {
                //TODO: confirm email
                Message = "User created successfully",
                IsSuccess = true
            };
        
        return new AuthManagerResponse
        {
            Message = "User did not created",
            IsSuccess = false,
            Errors = createUserResult.Errors.Select(e => e.Description).Concat(addRoleResult.Errors.Select(e => e.Description)).ToList()
        };
    }

    public async Task<AuthManagerResponse> LoginUserAsync(UserLoginRequest userRequest)
    {
        var user = await _userManager.FindByEmailAsync(userRequest.Email);
        if (user == null)
            return new AuthManagerResponse
            {
                Message = $"User with email '{userRequest.Email}' not found",
                IsSuccess = false
            };
        var result = await _userManager.CheckPasswordAsync(user, userRequest.Password);
        if (!result)
            return new AuthManagerResponse
            {
                Message = "Invalid password",
                IsSuccess = false
            };

        var claims = new List<Claim>();
        var token = await GenerateAccessToken(user, claims);
        var refreshToken = await GenerateRefreshToken(user, _configuration["AuthSettings:ApiTokenProvider"]);
        await _userManager.SetAuthenticationTokenAsync(user, _configuration["AuthSettings:ApiTokenProvider"], "RefreshToken", refreshToken);
        
        var tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);
        return new AuthLoginResponse
        {
            Message = "User has been successfully authenticated",
            IsSuccess = true,
            AcssessToken = tokenAsString,
            RefreshToken = refreshToken,
            ExpireDate = token.ValidTo
        };
    }

    public async Task<AuthManagerResponse> RefreshTokenAsync(string refreshToken, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new AuthManagerResponse
            {
                Message = $"User with id '{userId}' not found",
                IsSuccess = false
            };
        
        var result = await _userManager.VerifyUserTokenAsync(user, _configuration["AuthSettings:ApiTokenProvider"], "RefreshToken", refreshToken);
        if (!result)
            return new AuthManagerResponse
            {
                Message = "Refresh token not valid",
                IsSuccess = false
            };

        var claims = await _userManager.GetClaimsAsync(user);
        var token = await GenerateAccessToken(user, claims);
        var newRefreshToken = await GenerateRefreshToken(user, _configuration["AuthSettings:ApiTokenProvider"]);
        await _userManager.SetAuthenticationTokenAsync(user, _configuration["AuthSettings:ApiTokenProvider"], "RefreshToken", newRefreshToken);

        var tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);
        return new AuthLoginResponse
        {
            Message = "Token successfully refreshed",
            IsSuccess = true,
            AcssessToken = tokenAsString,
            RefreshToken = newRefreshToken,
            ExpireDate = token.ValidTo
        };
    }
    
    public async Task<AuthManagerResponse> ExternalLoginAsync(ExternalLoginInfo info)
    {
        var signinResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true);
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var user = await _userManager.FindByEmailAsync(email);
        var claims = user != null ? await _userManager.GetClaimsAsync(user) : new List<Claim>();

        if (signinResult.Succeeded && user != null)
        {
            var token = await GenerateAccessToken(user, claims);
            var refreshToken = await GenerateRefreshToken(user, info.ProviderDisplayName);
            await _userManager.SetAuthenticationTokenAsync(user, info.ProviderDisplayName, "RefreshToken",refreshToken);

            return new AuthLoginResponse
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
                    EmailConfirmed = true,
                    Basket = new Data.Api.Entities.Users.Basket()
                };
                var createUserResult = await _userManager.CreateAsync(user);
                var addRoleResult = await _userManager.AddToRoleAsync(user, _configuration["AuthSettings:DefaultUserRole"]);
                if (!createUserResult.Succeeded || !addRoleResult.Succeeded)
                    return new AuthManagerResponse()
                    {
                        Message = "Failed to add a user in database",
                        IsSuccess = false,
                        Errors = createUserResult.Errors.Select(e => e.Description).Concat(addRoleResult.Errors.Select(e => e.Description)).ToList()
                    };
            }

            await _userManager.AddLoginAsync(user, info);
            await _signInManager.SignInAsync(user, false);

            var token = await GenerateAccessToken(user, claims);
            var refreshToken = await GenerateRefreshToken(user, info.ProviderDisplayName);
            await _userManager.SetAuthenticationTokenAsync(user, info.ProviderDisplayName, "RefreshToken", refreshToken);

            return new AuthLoginResponse
            {
                AcssessToken = new JwtSecurityTokenHandler().WriteToken(token),
                ExpireDate = token.ValidTo,
                RefreshToken = refreshToken,
                Message = "User successfully Sign In from the external system",
                IsSuccess = true
            };
        }
        //TODO: add errors response
        return new AuthManagerResponse()
        {
            Message = "Failed to create a user from an external system",
            IsSuccess = false
        };
    }

    private async Task<JwtSecurityToken> GenerateAccessToken(IdentityUser user, ICollection<Claim> claims)
    {
        claims.Add(new Claim("email", user.Email));
        claims.Add(new Claim("sub", user.Id));
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

    private async Task<string> GenerateRefreshToken(IdentityUser user, string provider)
    {
        //TODO: set expiration date 
        await _userManager.RemoveAuthenticationTokenAsync(user, provider, "RefreshToken");
        var newRefreshToken = await _userManager.GenerateUserTokenAsync(user, provider,"RefreshToken");
        return newRefreshToken;
    }
}