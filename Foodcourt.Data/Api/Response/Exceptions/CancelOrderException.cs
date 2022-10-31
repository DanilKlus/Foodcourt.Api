using System.Net;
using Foodcourt.Data.Api.Entities.Orders;

namespace Foodcourt.Data.Api.Response.Exceptions;

public class CancelOrderException : Exception
{
    public string Message;
    public OrderStatus Status;
    public HttpStatusCode ErrorStatus;
    
    public CancelOrderException(string message, OrderStatus status)
    {
        Message = message;
        Status = status;
        ErrorStatus = HttpStatusCode.BadRequest;
    }
}