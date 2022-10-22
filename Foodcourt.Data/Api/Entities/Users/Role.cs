namespace Foodcourt.Data.Api.Entities.Users;

public class Role
{
    public long Id { get; set; }
    public string Name { get; set; }

    public virtual List<AppUser> AppUsers { get; set; }
}