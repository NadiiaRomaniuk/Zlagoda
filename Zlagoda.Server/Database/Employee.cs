namespace Zlagoda.Server.Database;

public class Employee
{
	public string EmployeeId { get; set; }
	public string Surname { get; set; }
    public string Name { get; set; }
    public string? Patronymic { get; set; }
    public string Role { get; set; }
	public decimal Salary { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public DateOnly DateOfStart { get; set; }
    public string PhoneNumber { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public string ZipCode { get; set; }
    public string? Password { get; set; }
}
