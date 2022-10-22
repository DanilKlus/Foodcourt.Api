using System.Net.Mime;
using Foodcourt.BusinessLogic.Services.Cafes;
using Foodcourt.Data.Api;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Foodcourt.Data.Api.Response.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Foodcourt.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("v1.0/[controller]")]
    public class CafesController : ControllerBase
    {
        private readonly ICafeService _cafeService;
        public CafesController(ICafeService cafeService) => 
            _cafeService = cafeService;

        [HttpGet]
        [ProducesResponseType(typeof(SearchResponse<CafeResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult> SearchCafes([FromQuery]CafeSearchRequest request)
        {
            var response = await _cafeService.SearchByQuery(request);
            return Ok(response);
        }
        
        [HttpGet("{cafeId:long}")]
        [ProducesResponseType(typeof(CafeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetCafe(long cafeId)
        {
            try {
                var response = await _cafeService.GetAsync(cafeId);
                return Ok(response);
            }
            catch (NotFoundException e) { return NotFound(e); }
        }
        
        [HttpGet("{cafeId:long}/products")]
        [ProducesResponseType(typeof(SearchResponse<ProductResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetCafeProducts(long cafeId)
        {
            var response = await _cafeService.GetProductsAsync(cafeId);
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
    }
}