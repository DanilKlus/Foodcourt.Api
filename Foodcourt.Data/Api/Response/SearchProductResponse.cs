using Foodcourt.Data.Api.Entities.Cafes;

namespace Foodcourt.Data.Api.Response;

public class SearchProductResponse
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Avatar { get; set; }
    public double Price  { get; set; }
    public double Weight { get; set; }
}