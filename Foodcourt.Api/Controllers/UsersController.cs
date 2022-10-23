using System.Net.Mime;
using Foodcourt.BusinessLogic.Services.Users;
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
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public UsersController(IUserService userService, SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager)
        {
            _userService = userService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(UserManagerResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> Register([FromBody] UserRegisterRequest registerRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var result = await _userService.RegisterUserAsync(registerRequest);
            return Ok(result);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(UserManagerResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> Login([FromBody] UserLoginRequest loginRequest)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await _userService.LoginUserAsync(loginRequest);
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("token/refresh")]
        [ProducesResponseType(typeof(UserManagerResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> RefreshLogin([FromBody] RefreshTokenRequest refreshRequest)
        {
            if (!ModelState.IsValid) return BadRequest();
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");

            var result = await _userService.RefreshLoginAsync(refreshRequest.RefreshToken, userId);
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("account/external-login")]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = $"https://localhost:7003/v1.0/users/account/external-auth-callback?returnUrl={returnUrl}";
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            properties.AllowRefresh = true;
            return Challenge(properties, provider);
        }


        //TODO refactor
        [HttpGet]
        [AllowAnonymous]
        [Route("account/external-auth-callback")]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return Unauthorized("Eailure when connecting to an external service");
            var result = await _userService.ExternalLogin(info);

            Response.Cookies.Delete("ApiToken");
            Response.Cookies.Append(
                "ApiTokens",
                JsonConvert.SerializeObject(result, new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    },
                    Formatting = Formatting.Indented
                }));
            return Redirect($"http://localhost:3000");
        }
    }
}