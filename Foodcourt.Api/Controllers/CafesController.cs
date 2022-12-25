using System.Net.Mime;
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
    public class CafesController : ControllerBase
    {
        private readonly ICafeService _cafeService;
        private readonly UserManager<IdentityUser> _userManager;
        public CafesController(ICafeService cafeService, UserManager<IdentityUser> userManager)
        {
            _cafeService = cafeService;
            _userManager = userManager;
        }
        
        [HttpGet("search")]
        [ProducesResponseType(typeof(SearchResponse<CafeResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult> Serach([FromQuery]CafeSearchRequest request)
        {
            var response = await _cafeService.SearchAsync(request);
            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(SearchResponse<CafeResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetCafes([FromQuery]CafeSearchRequest request)
        {
            var response = await _cafeService.GetCafesAsync(request);
            return Ok(response);
        }
        
        [Authorize(Roles = CustomRoles.Administrator)]
        [HttpGet("applications")]
        [ProducesResponseType(typeof(SearchResponse<CafeApplicationResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetCafesApplications([FromQuery]SearchApplicationRequest request)
        {
            if (!ModelState.IsValid) return BadRequest("Model not valid");
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");
            
            var response = await _cafeService.GetCafesApplicationsAsync(request, true, userId);
            return Ok(response);
        }
        
        [Authorize(Roles = CustomRoles.Director)]
        [HttpGet("my-applications")]
        [ProducesResponseType(typeof(SearchResponse<CafeApplicationResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetMyCafesApplications([FromQuery]SearchApplicationRequest request)
        {
            if (!ModelState.IsValid) return BadRequest("Model not valid");
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");
            
            var response = await _cafeService.GetCafesApplicationsAsync(request, false, userId);
            return Ok(response);
        }
        
        [HttpGet("{cafeId:long}")]
        [ProducesResponseType(typeof(CafeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetCafe(long cafeId)
        {
            try {
                var response = await _cafeService.GetCafeAsync(cafeId);
                return Ok(response);
            }
            catch (NotFoundException e) { return NotFound(e.Message); }
        }
        
        [HttpGet("{cafeId:long}/products")]
        [ProducesResponseType(typeof(SearchResponse<ProductResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetCafeProducts(long cafeId, [FromQuery] SearchRequest searchRequest)
        {
            var response = await _cafeService.GetProductsAsync(cafeId, searchRequest);
            return Ok(response);
        }

        [HttpGet("{cafeId:long}/products/{productId:long}")]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetCafeProduct(long cafeId, long productId)
        {
            try {
                var response = await _cafeService.GetProductAsync(cafeId, productId);
                return Ok(response);
            }
            catch (NotFoundException e) { return NotFound(e.Message); }
        }
        
        [HttpPost]
        [ProducesResponseType(typeof(CafeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]        
        public async Task<ActionResult> CreateCafe([FromBody]CafeCreateRequest request)
        {
            if (!ModelState.IsValid) return BadRequest("Model not valid");
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");
            
            await _cafeService.AddCafeAsync(request, userId);
            return Created("/cafes", "cafe created");
        }
        
        [Authorize(Roles = CustomRoles.Administrator)]
        [HttpPost("{cafeId:long}/approve")]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]        
        public async Task<ActionResult> ApproveCafe(long cafeId, [FromQuery] string responce)
        {
            if (!ModelState.IsValid) return BadRequest("Model not valid");
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");
            
            await _cafeService.SetCafeStatusAsync(cafeId, true, responce);
            return Ok();
        }
        
        [Authorize(Roles = CustomRoles.Administrator)]
        [HttpPost("{cafeId:long}/reject")]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]        
        public async Task<ActionResult> RejectCafe(long cafeId, [FromQuery] string responce)
        {
            if (!ModelState.IsValid) return BadRequest("Model not valid");
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");
            
            await _cafeService.SetCafeStatusAsync(cafeId, false, responce);
            return Ok();
        }
        
        [HttpPatch("{cafeId:long}")]
        [Authorize(Roles = CustomRoles.Director)]
        [ProducesResponseType(typeof(CafeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]        
        public async Task<ActionResult> PatchCafe([FromBody]PatchCafeRequest request, long cafeId)
        {
            if (!ModelState.IsValid) return BadRequest("Model not valid");
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");

            try
            {
                await _cafeService.PatchCafeAsync(request, userId, cafeId);
                return Ok();
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (NotHaveAccessException e)
            {
                return StatusCode(403, e.Message);
            }
        }

        [HttpDelete("{cafeId:long}")]
        [Authorize(Roles = CustomRoles.Director)]
        [ProducesResponseType(typeof(CafeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCafe(long cafeId)
        {
            if (!ModelState.IsValid) return BadRequest("Model not valid");
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");

            try
            {
                await _cafeService.DeleteCafeAsync(userId, cafeId);
                return Ok();
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (NotHaveAccessException e)
            {
                return StatusCode(403, e.Message);
            }
        }
        
        [HttpDelete("{cafeId:long}/products/{productId:long}")]
        [Authorize(Roles = CustomRoles.Director)]
        [ProducesResponseType(typeof(CafeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCafeProduct(long cafeId, long productId)
        {
            if (!ModelState.IsValid) return BadRequest("Model not valid");
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");

            try
            {
                await _cafeService.DeleteCafeProductAsync(userId, cafeId, productId);
                return Ok();
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (NotHaveAccessException e)
            {
                return StatusCode(403, e.Message);
            }
        }
        
        [HttpPost("{cafeId:long}/products")]
        [Authorize(Roles = CustomRoles.Director)]
        [ProducesResponseType(typeof(CafeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> CreateCafeProduct(long cafeId, [FromBody] CreateProductRequest request)
        {
            if (!ModelState.IsValid) return BadRequest("Model not valid");
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");

            try
            {
                await _cafeService.CreateCafeProductAsync(request, cafeId, userId);
                return Ok();
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (NotHaveAccessException e)
            {
                return StatusCode(403, e.Message);
            }
        }
    }
}