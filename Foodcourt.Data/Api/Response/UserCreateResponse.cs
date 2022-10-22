namespace Foodcourt.Data.Api.Response;

public class UserCreateResponse
{
    public string Message { get; set; }
    public bool IsSuccess { get; set; }
    public List<string> Errors { get; set; }
}