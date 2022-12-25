using Foodcourt.Data.Api.Entities.Cafes;

namespace Foodcourt.Data.Api.Response;
public class CafeApplicationResponse
{
    public long Id { get; set; }
    public string Name { get; set; }
    public DateTime CreationTime { get; set; }
    public CafeStatus Status { get; set; }
}