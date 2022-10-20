using Newtonsoft.Json;

namespace Foodcourt.Data.Entities.Users;

public class Role
{
    public long Id { get; set; }
    public string Name { get; set; }

    public List<User> Users { get; set; }
}