using System.Net.Mime;
using Foodcourt.BusinessLogic.Services.Orders;
using Foodcourt.Data.Api;
using Foodcourt.Data.Api.Entities.Orders;
using Foodcourt.Data.Api.Request;
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
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<IdentityUser> _userManager;
        
        public OrdersController(IOrderService orderService, UserManager<IdentityUser> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }

        
        [HttpPost]
        public async Task<ActionResult> CreateOrders()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");
            
            await _orderService.CreateOrders(userId);
            return Created("orders", userId);
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(SearchResponse<OrderResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult> SearchOrders([FromQuery] OrderStatus? status)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");
            
            var response = await _orderService.GetOrders(userId, status);
            return Ok(response);
        }
        
        [HttpGet("{orderId:long}")]
        [ProducesResponseType(typeof(SearchResponse<OrderResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetOrder(long orderId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");
            
            var response = await _orderService.GetOrder(userId, orderId);
            return Ok(response);
        }
        
        [HttpDelete("{orderId:long}")]
        public async Task<ActionResult> CancelOrder(long orderId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");
            
            await _orderService.CancelOrder(userId, orderId);
            return Ok();
        }
    }
}