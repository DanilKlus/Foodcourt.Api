using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Foodcourt.Data.Api.Entities.Users;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Foodcourt.BusinessLogic.Services.Cafes;

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
        var identityUser = new AppUser()
        {
            Email = userRequest.Email,
            UserName = userRequest.Email,
            PhoneNumber = userRequest.Phone,
            Name = userRequest.Name
        };

        var result = await _userManager.CreateAsync(identityUser, userRequest.Password);
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
            claims.Add(new Claim(ClaimTypes.Role, userRole));
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));
        var token = new JwtSecurityToken(
            issuer: _configuration["AuthSettings:Issuer"],
            audience: _configuration["AuthSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(Convert.ToDouble(_configuration["AuthSettings:TokenLifeDays"])),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        var tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);
        
        return new UserLoginResponse
        {
            Message = "User has been successfully authenticated",
            IsSuccess = true,
            AcssessToken = tokenAsString,
            RefreshToken = "not implented",
            ExpireDate = token.ValidTo
        }; 
    }
}