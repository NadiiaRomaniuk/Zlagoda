using MySqlConnector;

namespace Zlagoda.Server.Database;

public partial class Db
{
    public async Task<Customer?> GetCustomer(string id)
    {
        try
        {
            string query =
@"SELECT 
  card_number,
  cust_surname,
  cust_name,
  cust_patronymic,
  phone_number,
  city,
  street,
  zip_code,
  percent
FROM Customer_Card WHERE card_number = @id";
            var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@id", id);
            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    var customer = new Customer
                    {
                        CustomerId = reader.GetString(0),
                        Surname = reader.GetString(1),
                        Name = reader.GetString(2),
                        Patronymic = reader.IsDBNull(3) ? null : reader.GetString(3),
                        PhoneNumber = reader.GetString(4),
                        City = reader.IsDBNull(5) ? null : reader.GetString(5),
                        Street = reader.IsDBNull(6) ? null : reader.GetString(6),
                        ZipCode = reader.IsDBNull(7) ? null : reader.GetString(7),
                        Percent = reader.GetInt32(8),
                    };
                    return customer;
                }
                else
                    return null;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Db.GetCustomer error");
            return null;
        }
    }

    public async Task<List<CustomerShort>> GetCustomersShort()
    {
        try
        {
            var query =
@"SELECT
  card_number,
  CONCAT_WS(' ', cust_surname, cust_name, cust_patronymic) AS full_name,
  phone_number,
  percent
FROM Customer_Card";
            var command = new MySqlCommand(query, _connection);
            using (var reader = await command.ExecuteReaderAsync())
            {
                var list = new List<CustomerShort>();
                while (await reader.ReadAsync())
                {
                    var customer = new CustomerShort
                    {
                        CustomerId = reader.GetString(0),
                        FullName = reader.GetString(1),
                        PhoneNumber = reader.GetString(2),
                        Percent = reader.GetInt32(3),
                    };
                    list.Add(customer);
                }
                return list;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Db.GetCustomersShort error");
            return new List<CustomerShort>();
        }
    }

    public async Task<bool> AddCustomer(Customer customer)
    {
        try
        {
            var query =
@"INSERT INTO Customer_Card (
  card_number,
  cust_surname,
  cust_name,
  cust_patronymic,
  phone_number,
  city,
  street,
  zip_code,
  percent )
VALUES (
  @card_number,
  @cust_surname,
  @cust_name,
  @cust_patronymic,
  @phone_number,
  @city,
  @street,
  @zip_code,
  @percent )";
            var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@card_number", customer.CustomerId);
            command.Parameters.AddWithValue("@cust_surname", customer.Surname);
            command.Parameters.AddWithValue("@cust_name", customer.Name);
            command.Parameters.AddWithValue("@cust_patronymic", customer.Patronymic);
            command.Parameters.AddWithValue("@phone_number", customer.PhoneNumber);
            command.Parameters.AddWithValue("@city", customer.City);
            command.Parameters.AddWithValue("@street", customer.Street);
            command.Parameters.AddWithValue("@zip_code", customer.ZipCode);
            command.Parameters.AddWithValue("@percent", customer.Percent);
            await command.ExecuteScalarAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Db.AddCustomer error");
            return false;
        }
    }

    public async Task<bool> UpdateCustomer(Customer customer)
    {
        try
        {
            var query =
@"UPDATE Customer_Card SET
  cust_surname =    @cust_surname,
  cust_name =       @cust_name,
  cust_patronymic = @cust_patronymic,
  phone_number =    @phone_number,
  city =            @city,
  street =          @street,
  zip_code =        @zip_code,
  percent =         @percent
WHERE card_number = @card_number";
            var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@card_number", customer.CustomerId);
            command.Parameters.AddWithValue("@cust_surname", customer.Surname);
            command.Parameters.AddWithValue("@cust_name", customer.Name);
            command.Parameters.AddWithValue("@cust_patronymic", customer.Patronymic);
            command.Parameters.AddWithValue("@phone_number", customer.PhoneNumber);
            command.Parameters.AddWithValue("@city", customer.City);
            command.Parameters.AddWithValue("@street", customer.Street);
            command.Parameters.AddWithValue("@zip_code", customer.ZipCode);
            command.Parameters.AddWithValue("@percent", customer.Percent);
            await command.ExecuteScalarAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Db.AddCustomer error");
            return false;
        }
    }

    public async Task<bool> DeleteCustomer(string id)
    {
        try
        {
            var query =
@"DELETE FROM Customer_Card
WHERE card_number = @card_number";
            var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@card_number", id);
            await command.ExecuteScalarAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Db.DeleteCustomer error");
            return false;
        }
    }
}
