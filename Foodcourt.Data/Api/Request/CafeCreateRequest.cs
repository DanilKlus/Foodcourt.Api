namespace Foodcourt.Data.Api.Request;

public class CafeCreateRequest
{
    public string Name { get; set; }
    public string Adress { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string CertifyingDocument { get; set; }
    public string PersonalAccount { get; set; }
}