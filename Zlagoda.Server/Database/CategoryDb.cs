using MySqlConnector;

namespace Zlagoda.Server.Database;

public partial class Db
{
    public async Task<Category?> GetCategory(int id)
    {
        try
        {
            string query =
@"SELECT 
  category_number,
  category_name
FROM Category WHERE category_number = @id";
            var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@id", id);
            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    var category = new Category
                    {
                        CategoryId = reader.GetInt32(0),
                        Name = reader.GetString(1),
                    };
                    return category;
                }
                else
                    return null;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Db.GetCategory error");
            return null;
        }
    }

    public async Task<List<Category>> GetCategories()
    {
        try
        {
            var query =
@"SELECT
  category_number,
  category_name
FROM Category";
            var command = new MySqlCommand(query, _connection);
            using (var reader = await command.ExecuteReaderAsync())
            {
                var list = new List<Category>();
                while (await reader.ReadAsync())
                {
                    var category = new Category
                    {
                        CategoryId = reader.GetInt32(0),
                        Name = reader.GetString(1),
                    };
                    list.Add(category);
                }
                return list;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Db.GetCategories error");
            return new List<Category>();
        }
    }

    public async Task<int?> AddCategory(Category category)
    {
        try
        {
            var query =
@"INSERT INTO Category ( category_name )
VALUES ( @category_name );
SELECT LAST_INSERT_ID();";
            var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@category_name", category.Name);
            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Db.AddCategory error");
            return null;
        }
    }

    public async Task<bool> UpdateCategory(Category category)
    {
        try
        {
            var query =
@"UPDATE Category
SET category_name = @category_name
WHERE category_number = @category_number";
            var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@category_number", category.CategoryId);
            command.Parameters.AddWithValue("@category_name", category.Name);
            await command.ExecuteScalarAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Db.UpdateCategory error");
            return false;
        }
    }

    public async Task<bool> DeleteCategory(int id)
    {
        try
        {
            var query =
@"DELETE FROM Category
WHERE category_number = @category_number";
            var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@category_number", id);
            await command.ExecuteScalarAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Db.DeleteCategory error");
            return false;
        }
    }
}
