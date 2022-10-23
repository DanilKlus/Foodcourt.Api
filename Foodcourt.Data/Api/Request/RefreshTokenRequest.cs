using System.ComponentModel.DataAnnotations;

namespace Foodcourt.Data.Api.Request;

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; }
}