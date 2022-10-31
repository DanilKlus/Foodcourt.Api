using System.Net;

namespace Foodcourt.Data.Api.Response.Exceptions;

public class AddProductException : Exception
{
    public string Message;
    public long ProductId;
    public HttpStatusCode ErrorStatus;
    
    public AddProductException(string message, long productId)
    {
        Message = message;
        ProductId = productId;
        ErrorStatus = HttpStatusCode.BadRequest;
    }
}