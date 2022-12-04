using System.Net.Mime;
using Foodcourt.BusinessLogic.Services.Orders;
using Foodcourt.Data.Api;
using Foodcourt.Data.Api.Entities.Orders;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;
using Foodcourt.Data.Api.Response.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Foodcourt.Api.Controllers
{
    //TODO: add method: payOrder
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
            
            await _orderService.CreateOrdersAsync(userId);
            return Created("orders", userId);
        }
        
        [HttpPatch("{orderId:long}")]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> PathOrder([FromBody] PathOrderRequest patchRequest, long orderId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");
            
            try {
                var result = await _orderService.PatchOrderAsync(userId, orderId, patchRequest);
                return Ok(result);
            }
            catch (NotFoundException e) { return NotFound(e.Message); }
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(SearchResponse<OrderResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult> SearchOrders([FromQuery] OrderStatus? status, [FromQuery] SearchRequest searchRequest)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");
            
            var response = await _orderService.GetOrdersAsync(userId, status, searchRequest);
            return Ok(response);
        }
        
        [HttpGet("{orderId:long}")]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetOrder(long orderId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");
            
            try {
                var response = await _orderService.GetOrderAsync(userId, orderId);
                return Ok(response);
            }
            catch (NotFoundException e) { return NotFound(e.Message); }
        }
        
        [HttpPost("pay")]
        public async Task<ActionResult> PayOrders([FromQuery] List<long> ordersIds)
        {
            //mock method
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");
            
            await _orderService.PayOrdersAsync(userId, ordersIds);
            return Ok();
        }
        
        //TODO: add payment and push
        [HttpDelete("{orderId:long}")]
        public async Task<ActionResult> CancelOrder(long orderId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");
            
            try {
                await _orderService.CancelOrderAsync(userId, orderId);
                return Ok();
            }
            catch (CancelOrderException e) { return BadRequest(e.Message); }
        }
        
        [HttpPost("{orderId:long}/repeat")]
        public async Task<ActionResult> RepeatOrder(long orderId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest("User does not have ID");
            
            try {
                await _orderService.RepeatOrderAsync(userId, orderId);
                return Created($"orders/{orderId}/repeat", "created");
            }
            catch (NotFoundException e) { return NotFound(e.Message); }
            catch (AddProductException e) { return BadRequest(e.Message); }
        }
    }
}