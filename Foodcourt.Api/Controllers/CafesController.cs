using System.Net.Mime;
using Foodcourt.BusinessLogic.Services.Cafes;
using Foodcourt.Data.Api;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Foodcourt.Data.Api.Response.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Foodcourt.Api.Controllers
{
    [Authorize(Roles = "user")]
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

        [HttpGet]
        [ProducesResponseType(typeof(SearchResponse<CafeResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult> SearchCafes([FromQuery]CafeSearchRequest request)
        {
            var response = await _cafeService.GetCafesAsync(request);
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
            catch (NotFoundException e) { return NotFound(e); }
        }
        
        [HttpGet("{cafeId:long}/products")]
        [ProducesResponseType(typeof(SearchResponse<ProductResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetCafeProducts(long cafeId, [FromQuery] string? query, 
            [FromQuery] int? skip, [FromQuery] int? take)
        {
            var response = await _cafeService.GetProductsAsync(cafeId, query, skip, take);
            return Ok(response);
        }
        
        [HttpGet("{cafeId:long}/products/{productId:long}")]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetCafeProducts(long cafeId, long productId)
        {
            try {
                var response = await _cafeService.GetProductAsync(cafeId, productId);
                return Ok(response);
            }
            catch (NotFoundException e) { return NotFound(e); }
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
    }
}