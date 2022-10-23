namespace Foodcourt.Data.Api.Response;

public class UserLoginResponse : UserManagerResponse
{
    public string AcssessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime? ExpireDate { get; set; }
}