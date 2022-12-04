using System.Net;

namespace Foodcourt.Data.Api.Response.Exceptions;

public class NotHaveAccessException : Exception
{
    public string Message;
    public HttpStatusCode ErrorStatus;
    
    public NotHaveAccessException(string message)
    {
        Message = message;
        ErrorStatus = HttpStatusCode.NotExtended;
    }
}