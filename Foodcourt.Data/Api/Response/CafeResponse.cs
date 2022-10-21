using Foodcourt.Data.Api.Entities.Cafes;

namespace Foodcourt.Data.Api.Response;

public class CafeResponse
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public CafeStatus Status { get; set; }
    public string Avatar { get; set; }
    public string Location { get; set; }
}