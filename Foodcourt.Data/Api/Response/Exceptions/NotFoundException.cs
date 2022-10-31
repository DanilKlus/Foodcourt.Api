using System.Net;

namespace Foodcourt.Data.Api.Response.Exceptions;

public class NotFoundException : Exception
{
    public string Message;
    public HttpStatusCode ErrorStatus;
    
    public NotFoundException(string message)
    {
        Message = message;
        ErrorStatus = HttpStatusCode.NotExtended;
    }
}