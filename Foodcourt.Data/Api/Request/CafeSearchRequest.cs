using Foodcourt.Data.Entities.Cafes;

namespace Foodcourt.Data.Api.Request;

public class CafeSearchRequest
{
    public string? Query { get; set; }
    public string? Name { get; set; }
    public CafeStatus? Status { get; set; }
    public string? Location { get; set; }
    public bool? IsActive { get; set; }
    public int? Skip { get; set; }
    public int? Take { get; set; }
}