namespace Zlagoda.Server.Models;

public class UserLoginModel
{
    public string Login { get; set; }
    public string Name { get; set; }
    public string[] Roles { get; set; }
}
