using Foodcourt.Data.Api.Entities.Cafes;

namespace Foodcourt.Data.Api.Response;

public class CafeResponse
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Distance { get; set; }
    public string Avatar { get; set; }
    public string Adress { get; set; }
    public double Rating { get; set; }
}