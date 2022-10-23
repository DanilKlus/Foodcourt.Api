using System.Net.Mime;
using Foodcourt.BusinessLogic.Services.Cafes;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Foodcourt.Api.Controllers
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("v1.0/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly SignInManager<IdentityUser> _signInManager;
        public UsersController(IUserService userService, SignInManager<IdentityUser> signInManager)
        {
            _userService = userService;
            _signInManager = signInManager;
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
        
        // [HttpPost]
        // [AllowAnonymous]
        // [Route("account/external-login")]
        // public IActionResult ExternalLogin(string provider, string returnUrl)
        // {
        //     var redirectUrl = $"https://localhost:7003/v1.0/cafes";
        //     var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        //     properties.AllowRefresh = true;
        //     return Challenge(properties, provider);
        // }
    }
}