namespace Zlagoda.Server.Database;

public class EmployeeShort
{
    public string EmployeeId { get; set; }
    public string FullName { get; set; }
    public string Role { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string PhoneNumber { get; set; }
}
