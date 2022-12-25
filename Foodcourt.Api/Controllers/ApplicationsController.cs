using System.Net.Mime;
using Foodcourt.BusinessLogic.Services.Applications;
using Foodcourt.BusinessLogic.Services.Cafes;
using Foodcourt.Data.Api;
using Foodcourt.Data.Api.Entities.Users;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Foodcourt.Data.Api.Response.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Foodcourt.Api.Controllers
{
    [Authorize(Roles = CustomRoles.User)]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("v1.0/[controller]")]
    public class ApplicationsController : ControllerBase
    {
        private readonly IApplicationsService _applicationsService;
        private readonly UserManager<IdentityUser> _userManager;
        public ApplicationsController(IApplicationsService applicationsService, UserManager<IdentityUser> userManager)
        {
            _applicationsService = applicationsService;
            _userManager = userManager;
        }

        [Authorize(Roles = CustomRoles.Administrator)]
        [HttpGet]
        [ProducesResponseType(typeof(SearchResponse<CafeApplicationResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetCafesApplications([FromQuery]SearchApplicationRequest request)
        {
            if (!ModelState.IsValid) return BadRequest("Model not valid");
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");
            
            var response = await _applicationsService.GetCafesApplicationsAsync(request);
            return Ok(response);
        }
        
        [HttpGet("my")]
        [ProducesResponseType(typeof(SearchResponse<CafeApplicationResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetMyCafesApplications()
        {
            if (!ModelState.IsValid) return BadRequest("Model not valid");
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");
            
            var response = await _applicationsService.GetMyCafesApplicationsAsync(userId);
            return Ok(response);
        }
        
        [HttpGet("my/{cafeId:long}")]
        [ProducesResponseType(typeof(CafeApplicationResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetMyCafeApplication(long cafeId)
        {
            if (!ModelState.IsValid) return BadRequest("Model not valid");
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");

            try
            {
                var response = await _applicationsService.GetMyCafeApplicationAsync(userId, cafeId);
                return Ok(response);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [Authorize(Roles = CustomRoles.Administrator)]
        [HttpPost("{cafeId:long}/approve")]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]        
        public async Task<ActionResult> ApproveCafe(long cafeId, [FromQuery] string response)
        {
            if (!ModelState.IsValid) return BadRequest("Model not valid");
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");
            
            try
            {
                await _applicationsService.SetCafeStatusAsync(cafeId, true, response);
                return Ok();
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }
        
        [Authorize(Roles = CustomRoles.Administrator)]
        [HttpPost("{cafeId:long}/reject")]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]        
        public async Task<ActionResult> RejectCafe(long cafeId, [FromQuery] string response)
        {
            if (!ModelState.IsValid) return BadRequest("Model not valid");
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");

            try
            {
                await _applicationsService.SetCafeStatusAsync(cafeId, false, response);
                return Ok();
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }
    }
}