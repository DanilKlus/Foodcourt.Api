using Foodcourt.Data.Api.Entities.Cafes;

namespace Foodcourt.Data.Api.Response;
public class CafeApplicationResponse
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Avatar { get; set; }
    public string Address { get; set; }
    public string Responce { get; set; }
    public CafeStatus Status { get; set; }
    public string CertifyingDocument { get; set; }
}