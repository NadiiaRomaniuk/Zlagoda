using MySqlConnector;

namespace Zlagoda.Server.Database;

public partial class Db
{
    public async Task<int> GetEmployeesCount()
    {
        try
        {
            var query = "SELECT COUNT(*) FROM Employee";
            var command = new MySqlCommand(query, _connection);
            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Db.GetEmployeesCount error");
            return 0;
        }
    }

    public async Task<Employee?> GetEmployee(string id)
    {
        try
        {
            string query =
@"SELECT 
  id_employee,
  empl_surname,
  empl_name,
  empl_patronymic,
  empl_role,
  salary,
  date_of_birth,
  date_of_start,
  phone_number,
  city,
  street,
  zip_code,
  password
FROM Employee WHERE id_employee = @id";
            var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@id", id);
            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    var employee = new Employee
                    {
                        EmployeeId = reader.GetString(0),
                        Surname = reader.GetString(1),
                        Name = reader.GetString(2),
                        Patronymic = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Role = reader.GetString(4),
                        Salary = reader.GetDecimal(5),
                        DateOfBirth = reader.GetDateOnly(6),
                        DateOfStart = reader.GetDateOnly(7),
                        PhoneNumber = reader.GetString(8),
                        City = reader.GetString(9),
                        Street = reader.GetString(10),
                        ZipCode = reader.GetString(11),
                        Password = reader.GetString(12),
                    };
                    return employee;
                }
                else
                    return null;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Db.GetEmploee error");
            return null;
        }
    }

    public async Task<List<EmployeeShort>> GetEmployeesShort()
    {
        try
        {
            var query =
@"SELECT
  id_employee,
  CONCAT_WS(' ', empl_surname, empl_name, empl_patronymic) AS full_name,
  empl_role,
  date_of_birth,
  phone_number
FROM Employee";
            var command = new MySqlCommand(query, _connection);
            using (var reader = await command.ExecuteReaderAsync())
            {
                var list = new List<EmployeeShort>();
                while (await reader.ReadAsync())
                {
                    var employee = new EmployeeShort
                    {
                        EmployeeId = reader.GetString(0),
                        FullName = reader.GetString(1),
                        Role = reader.GetString(2),
                        DateOfBirth = reader.GetDateOnly(3),
                        PhoneNumber = reader.GetString(4),
                    };
                    list.Add(employee);
                }
                return list;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Db.GetEmployeesShort error");
            return new List<EmployeeShort>();
        }
    }

    public async Task<bool> AddEmployee(Employee employee)
    {
        try
        {
            var query =
@"INSERT INTO Employee (
  id_employee,
  empl_surname,
  empl_name,
  empl_patronymic,
  empl_role,
  salary,
  date_of_birth,
  date_of_start,
  phone_number,
  city,
  street,
  zip_code,
  password )
VALUES (
  @id_employee,
  @empl_surname,
  @empl_name,
  @empl_patronymic,
  @empl_role,
  @salary,
  @date_of_birth,
  @date_of_start,
  @phone_number,
  @city,
  @street,
  @zip_code,
  @password )";
            var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@id_employee", employee.EmployeeId);
            command.Parameters.AddWithValue("@empl_surname", employee.Surname);
            command.Parameters.AddWithValue("@empl_name", employee.Name);
            command.Parameters.AddWithValue("@empl_patronymic", employee.Patronymic);
            command.Parameters.AddWithValue("@empl_role", employee.Role);
            command.Parameters.AddWithValue("@salary", employee.Salary);
            command.Parameters.AddWithValue("@date_of_birth", employee.DateOfBirth);
            command.Parameters.AddWithValue("@date_of_start", employee.DateOfStart);
            command.Parameters.AddWithValue("@phone_number", employee.PhoneNumber);
            command.Parameters.AddWithValue("@city", employee.City);
            command.Parameters.AddWithValue("@street", employee.Street);
            command.Parameters.AddWithValue("@zip_code", employee.ZipCode);
            command.Parameters.AddWithValue("@password", employee.Password);
            await command.ExecuteScalarAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Db.AddEmployee error");
            return false;
        }
    }

    public async Task<bool> UpdateEmployee(Employee employee, bool savePass)
    {
        try
        {
            var query =
@$"UPDATE Employee SET
  empl_surname =     @empl_surname,
  empl_name =        @empl_name,
  empl_patronymic =  @empl_patronymic,
  empl_role =        @empl_role,
  salary =           @salary,
  date_of_birth =    @date_of_birth,
  date_of_start =    @date_of_start,
  phone_number =     @phone_number,
  city =             @city,
  street =           @street,
  {(savePass ? "password = @password," : "")}
  zip_code =         @zip_code
WHERE id_employee = @id_employee";
            var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@id_employee", employee.EmployeeId);
            command.Parameters.AddWithValue("@empl_surname", employee.Surname);
            command.Parameters.AddWithValue("@empl_name", employee.Name);
            command.Parameters.AddWithValue("@empl_patronymic", employee.Patronymic);
            command.Parameters.AddWithValue("@empl_role", employee.Role);
            command.Parameters.AddWithValue("@salary", employee.Salary);
            command.Parameters.AddWithValue("@date_of_birth", employee.DateOfBirth);
            command.Parameters.AddWithValue("@date_of_start", employee.DateOfStart);
            command.Parameters.AddWithValue("@phone_number", employee.PhoneNumber);
            command.Parameters.AddWithValue("@city", employee.City);
            command.Parameters.AddWithValue("@street", employee.Street);
            command.Parameters.AddWithValue("@zip_code", employee.ZipCode);
            if (savePass)
                command.Parameters.AddWithValue("@password", employee.Password);
            await command.ExecuteScalarAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Db.AddEmployee error");
            return false;
        }
    }

    public async Task<bool> DeleteEmployee(string id)
    {
        try
        {
            var query =
@"DELETE FROM Employee
WHERE id_employee = @id_employee";
            var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@id_employee", id);
            await command.ExecuteScalarAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Db.AddEmployee error");
            return false;
        }
    }
}
