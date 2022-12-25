using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Foodcourt.Data;
using Foodcourt.Data.Api.Entities.Users;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Net.Mail;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Console;


namespace Foodcourt.BusinessLogic.Services.Auth;

public class AuthService : IAuthService
{
    private SynchronousConsoleLog consoleLog = new SynchronousConsoleLog();
    private readonly AppDataContext _dataContext;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AuthService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, AppDataContext dataContext, 
        IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
        _signInManager = signInManager;
        _dataContext = dataContext;
    }


    public async Task<AuthManagerResponse> RegisterUserAsync(UserRegisterRequest userRequest)
    {
        consoleLog.Info($"Registration request:'{userRequest.Email}'.");
        var appUser = new AppUser()
        {
            Email = userRequest.Email,
            UserName = userRequest.Email,
            PhoneNumber = userRequest.Phone,
            Name = userRequest.Name,
            Basket = new Data.Api.Entities.Users.Basket(),
            EmailConfirmed = !Convert.ToBoolean(_configuration["Email:IsActive"])
        };
        var createUserResult = await _userManager.CreateAsync(appUser, userRequest.Password);
        var addRoleResult = await _userManager.AddToRoleAsync(appUser, _configuration["AuthSettings:DefaultUserRole"]);
        if (createUserResult.Succeeded && addRoleResult.Succeeded)
        {
            consoleLog.Info($"Registered user with email'{userRequest.Email}'.");
            if (Convert.ToBoolean(_configuration["Email:IsActive"]))
                await SendConfirmationCode(userRequest.Email);
            return new AuthManagerResponse
            {
                Message = "User created successfully",
                IsSuccess = true
            };
        }
        
        return new AuthManagerResponse
        {
            Message = "User did not created",
            IsSuccess = false,
            Errors = createUserResult.Errors.Select(e => e.Description).Concat(addRoleResult.Errors.Select(e => e.Description)).ToList()
        };
    }

    public async Task<AuthManagerResponse> LoginUserAsync(UserLoginRequest userRequest)
    {
        consoleLog.Info($"Authentication request:'{userRequest.Email}'.");
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
        var isConfirmed = await  _userManager.IsEmailConfirmedAsync(user);
        if (!isConfirmed)
            return new AuthManagerResponse
            {
                Message = "Email not confirmed",
                IsSuccess = false
            };

        var claims = new List<Claim>();
        var token = await GenerateAccessToken(user, claims);
        var refreshToken = await GenerateRefreshToken(user, _configuration["AuthSettings:ApiTokenProvider"]);
        await _userManager.SetAuthenticationTokenAsync(user, _configuration["AuthSettings:ApiTokenProvider"], "RefreshToken", refreshToken);
        
        var tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);
        consoleLog.Info($"Authenticated with email'{userRequest.Email}'.");
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
        return new AuthManagerResponse()
        {
            Message = "Failed to create a user from an external system",
            IsSuccess = false
        };
    }

    public async Task SendConfirmationCode(string email)
    {
        var user = await _dataContext.AppUsers.FirstOrDefaultAsync(x => x.Email.Equals(email));
        if (user == null)
            throw new Exception("user not found");
        var confirmed = await _userManager.IsEmailConfirmedAsync(user);
        if (confirmed)
            throw new Exception("email confirmed");
        
        var confirmationCode = new Random().Next(1000, 9999);
        var expiredTo = DateTime.UtcNow;
        
        user.ConfirmationCode = confirmationCode;
        user.CodeExpiredTo = expiredTo;
        await _userManager.UpdateAsync(user);
        
        var result = SendCode(email, confirmationCode, expiredTo);
    }
    
    //TODO: SRP (email service)
    private bool SendCode(string email, int confirmationCode, DateTime expiredTo) {
        try
        {
            MailMessage message = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            message.From = new MailAddress(_configuration["Email:Address"]);
            message.To.Add(new MailAddress(email));
            message.Subject = "Confirm your email";
            message.IsBodyHtml = false; //to make message body as html  
            message.Body = "your code: " + confirmationCode + " expired to: " + expiredTo;
            smtp.Port = 587;
            smtp.Host = "smtp.yandex.ru"; //for gmail host  
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(_configuration["Email:Address"], _configuration["Email:Password"]);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Send(message);
            return true;
        }
        catch (Exception)
        {
            return false;
        }  
    }  

    public async Task ConfirmCode(string email, int code)
    {
        var user = await _dataContext.AppUsers.FirstOrDefaultAsync(x => x.Email.Equals(email));
        if (user == null)
            throw new Exception("user not found");

        var confirmationCode = user.ConfirmationCode;
        var expiredTo = user.CodeExpiredTo;

        if (DateTime.Now.CompareTo(expiredTo) != 1)
            throw new Exception("token expired");
        if (!confirmationCode.Equals(code))
            throw new Exception("code not valid");

        user.EmailConfirmed = true;
        user.ConfirmationCode = 0;
        user.CodeExpiredTo = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
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
            expires: DateTime.Now.AddDays(Convert.ToDouble(15.0)),
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