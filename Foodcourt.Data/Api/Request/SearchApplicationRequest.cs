using Foodcourt.Data.Api.Entities.Cafes;

namespace Foodcourt.Data.Api.Request;

public class SearchApplicationRequest
{
    public string? Query { get; set; } = ""; 
    public CafeStatus? Status { get; set; }
    public int? Skip { get; set; }
    public int? Take { get; set; }
}