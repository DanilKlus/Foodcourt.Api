using Foodcourt.Data.Api.Entities.Cafes;

namespace Foodcourt.Data.Api.Response;

public class CafeResponse
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Distance { get; set; }
    public string Description { get; set; }
    public string Avatar { get; set; }
    public string Address { get; set; }
    public string Rating { get; set; }
}