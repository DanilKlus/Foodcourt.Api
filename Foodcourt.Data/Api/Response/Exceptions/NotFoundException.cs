using System.Net;

namespace Foodcourt.Data.Api.Response.Exceptions;

public class NotFoundException : Exception
{
    public string Message;
    public HttpStatusCode ErrorStatus;
    
    public NotFoundException(HttpStatusCode errorStatus, string message)
    {
        Message = message;
        ErrorStatus = errorStatus;
    }
}