using System.Net;
using Foodcourt.Data.Api.Entities.Orders;

namespace Foodcourt.Data.Api.Response.Exceptions;

public class CreateOrderException : Exception
{
    public string Message;
    public OrderStatus Status;
    public HttpStatusCode ErrorStatus;
    
    public CreateOrderException(string message)
    {
        Message = message;
        ErrorStatus = HttpStatusCode.BadRequest;
    }
}