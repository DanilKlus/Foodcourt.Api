using System.Net.Mime;
using System.Runtime.InteropServices;
using Foodcourt.BusinessLogic.Services.Cafes;
using Foodcourt.Data.Api;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Entities.Response;
using Microsoft.AspNetCore.Mvc;

namespace Foodcourt.Api.Controllers
{
    //[Authorize]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("v1.0/[controller]")]
    public class CafesController : ControllerBase
    {
        private readonly ICafeService cafeService;

        public CafesController(ICafeService cafeService)
        {
            this.cafeService = cafeService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(SearchResponse<CafeSearchResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FileNotFoundException), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> SearchCafes([FromQuery]CafeSearchRequest request)
        {
            var response = await cafeService.SearchByQuery(request);
            return Ok(response);
        }
        
        [HttpGet("{cafeId}")]
        [ProducesResponseType(typeof(CafeSearchResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetCafe(Guid cafeId)
        {
            var response = await cafeService.Get(cafeId);
            return Ok(response);
        }
    }
}