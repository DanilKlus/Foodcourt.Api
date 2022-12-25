namespace Foodcourt.Data.Api.Request;

public class SearchApplicationRequest
{
    public string? Query { get; set; } = ""; 
    public bool? Approved { get; set; }
    public bool? IsActive { get; set; }
    public int? Skip { get; set; }
    public int? Take { get; set; }
}