using Foodcourt.Data.Api.Entities.Cafes;

namespace Foodcourt.Data.Api.Response;
public class FullApplicationEntity
{
    public long Id { get; set; }
    public string Name { get; set; }
    public DateTime CreationTime { get; set; }
    public string Address { get; set; }
    public string Response { get; set; }
    public CafeStatus Status { get; set; }
    public string CertifyingDocument { get; set; }
    public string PersonalAccount { get; set; }
    public string Rating { get; set; }
    public string Description { get; set; }
}