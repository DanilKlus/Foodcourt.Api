using System.ComponentModel.DataAnnotations;

namespace Foodcourt.Data.Api.Request;

public class UserRegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Phone { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Password { get; set; }
}