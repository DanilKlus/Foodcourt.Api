namespace Foodcourt.Data.Api.Request;

public class SearchRequest
{
    public string? Query { get; set; } 
    public int? Skip { get; set; }
    public int? Take { get; set; }
}