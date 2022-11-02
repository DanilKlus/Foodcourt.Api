namespace Foodcourt.Data.Api.Request;

public class PatchUserRequest
{
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Name { get; set; }
    public string? OldPassword { get; set; }
    public string? NewPassword { get; set; }
}