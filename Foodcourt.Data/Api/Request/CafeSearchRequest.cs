namespace Foodcourt.Data.Api.Request;

public class CafeSearchRequest
{
    public string? Name { get; set; }
    public string? Location { get; set; }
    public int? Skip { get; set; }
    public int? Take { get; set; }
}