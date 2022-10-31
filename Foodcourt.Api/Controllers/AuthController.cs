using System.Net.Mime;
using Foodcourt.BusinessLogic.Services.Auth;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Foodcourt.Api.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("v1.0/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _authService = authService;
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("registration")]
        [ProducesResponseType(typeof(AuthManagerResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> Register([FromBody] UserRegisterRequest registerRequest)
        {
            if (!ModelState.IsValid) 
                return BadRequest();
            
            var result = await _authService.RegisterUserAsync(registerRequest);
            if (result.IsSuccess)
                return Created("auth/registration", result);

            return BadRequest(result);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthManagerResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> Login([FromBody] UserLoginRequest loginRequest)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await _authService.LoginUserAsync(loginRequest);
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(AuthManagerResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> RefreshLogin([FromBody] RefreshTokenRequest refreshRequest)
        {
            if (!ModelState.IsValid) return BadRequest("Model not valid");
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");

            var result = await _authService.RefreshTokenAsync(refreshRequest.RefreshToken, userId);
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("account/external-login")]
        public IActionResult ExternalLogin(string provider, string backUrl)
        {
            var redirectUrl = $"{_configuration["BaseUrl"]}/auth/account/external-auth-callback?returnUrl={backUrl}";
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            properties.AllowRefresh = true;
            
            return Challenge(properties, provider);
        }
        
        [HttpGet]
        [AllowAnonymous]
        [Route("account/external-auth-callback")]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return Unauthorized("Connecting to an external service failed");

            var result = await _authService.ExternalLoginAsync(info);
            if (!result.IsSuccess) return Unauthorized(result);

            Response.Cookies.Append("ApiTokens",
                JsonConvert.SerializeObject(result, new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver {NamingStrategy = new CamelCaseNamingStrategy()},
                    Formatting = Formatting.Indented
                }));
            return Redirect(_configuration["FrontDomain"]);
        }
    }
}