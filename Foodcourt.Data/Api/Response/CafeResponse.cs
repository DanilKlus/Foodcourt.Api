using Foodcourt.Data.Api.Entities.Cafes;

namespace Foodcourt.Data.Api.Response;

public class CafeResponse
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public CafeStatus Status { get; set; }
    public string Avatar { get; set; }
    public string Adress { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}