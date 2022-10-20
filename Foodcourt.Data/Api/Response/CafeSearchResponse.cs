using Foodcourt.Data.Entities.Cafes;

namespace Foodcourt.Data.Entities.Response;

public class CafeSearchResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public CafeStatus Status { get; set; }
    public string Avatar { get; set; }
    public string Location { get; set; }
}