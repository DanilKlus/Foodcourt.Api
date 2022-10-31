namespace Foodcourt.Data.Api.Request;

public class CafeSearchRequest
{
    public string? Name { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int? Skip { get; set; }
    public int? Take { get; set; }
}