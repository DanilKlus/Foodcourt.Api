using System.Net.Mime;
using Foodcourt.BusinessLogic.Services.Users;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Foodcourt.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "user")]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("v1.0/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserService _userService;
        public ProfileController(UserManager<IdentityUser> userManager, IUserService userService)
        {
            _userManager = userManager;
            _userService = userService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetUser()
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");;
            
            var result = await _userService.GetUserAsync(userId);
            return Ok(result);
        }
        
        [HttpPatch]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> PatchUser([FromBody] PatchUserRequest userRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");;
            
            var result = await _userService.PatchUserAsync(userId, userRequest);
            return Ok(result);
        }
        
        [HttpGet("faq")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetFaq()
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");;
            
            return Ok("Faq, blablabla");
        }
    }
}