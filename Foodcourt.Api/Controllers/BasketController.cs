using System.Net.Mime;
using Foodcourt.BusinessLogic.Services.Basket;
using Foodcourt.Data.Api.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Foodcourt.Api.Controllers
{
    [Authorize(Roles = "user")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("v1.0/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;
        private readonly UserManager<IdentityUser> _userManager;
        
        public BasketController(IBasketService basketService, UserManager<IdentityUser> userManager)
        {
            _basketService = basketService;
            _userManager = userManager;
        }

        [HttpGet]
        [ProducesResponseType(typeof(BasketResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetBasket()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");
            
            var response = await _basketService.GetBasket(userId);
            return Ok(response);
        }
    }
}