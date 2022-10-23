namespace Foodcourt.Data.Api.Response;

public class AuthLoginResponse : AuthManagerResponse
{
    public string AcssessToken { get; set; }
    public DateTime? ExpireDate { get; set; }
    public string RefreshToken { get; set; }
}