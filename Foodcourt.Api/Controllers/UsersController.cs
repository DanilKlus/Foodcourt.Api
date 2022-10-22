using System.Net.Mime;
using Foodcourt.BusinessLogic.Services.Cafes;
using Foodcourt.Data.Api;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Foodcourt.Data.Api.Response.Exceptions;
using Microsoft.AspNetCore.Mvc;
using UserRegisterRequest = Foodcourt.Data.Api.Request.UserRegisterRequest;

namespace Foodcourt.Api.Controllers
{
    //[Authorize]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("v1.0/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService) => 
            _userService = userService;

        [HttpPost("register")]
        [ProducesResponseType(typeof(UserRegisterRequest), StatusCodes.Status200OK)]
        public async Task<ActionResult> Register([FromBody] UserRegisterRequest registerRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var result = await _userService.RegisterUserAsync(registerRequest);
            return Ok(result);
        }
    }
}