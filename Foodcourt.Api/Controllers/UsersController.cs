using System.Net.Mime;
using Foodcourt.Data;
using Foodcourt.Data.Entities.Orders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Foodcourt.Api.Controllers;

[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Route("v1.0/[controller]")]
public class UsersController
{
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
                _context = context;
        }

        [HttpGet("{userId}/orders")]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Order>>> GetUserOrders(Guid userId)
        {
            var orders = await _context.Orders.Where(order => order.UserId == userId).ToListAsync();
            return orders;
        }

        // [HttpGet("{inquiryId}/interns")]
        // [ProducesResponseType(typeof(InternsResponse), StatusCodes.Status200OK)]
        // [ProducesResponseType(typeof(ServiceError), StatusCodes.Status404NotFound)]
        // [ProducesResponseType(typeof(ServiceError), StatusCodes.Status403Forbidden)]
        // public async Task<ActionResult<InternsResponse>> GetInterns([FromRoute] Guid inquiryId)
        // {
        //     var interns = await inquiryService.GetInquiryInterns(inquiryId);
        //     return Ok(new InternsResponse(interns.FoundEntities, interns.TotalCount));
        // }
}