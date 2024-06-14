using MySqlConnector;

namespace Zlagoda.Server.Database;

public partial class Db
{
    public async Task<Product?> GetProduct(int id)
    {
        try
        {
            string query =
@"SELECT 
  p.id_product,
  p.product_name, 
  p.category_number,
  p.characteristics
FROM 
  Product p
WHERE 
  p.id_product = @id;
";
            var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@id", id);
            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    var product = new Product
                    {
                        ProductId = reader.GetInt32(0),
                        ProductName = reader.GetString(1),
                        CategoryNumber = reader.GetInt32(2),
                        ProductCharacteristics = reader.GetString(3),
                    };
                    return product;
                }
                else
                    return null;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Db.GetProduct error");
            return null;
        }
    }

    public async Task<List<Product>> GetProducts()
    {
        try
        {
            var query =
@"SELECT 
  p.id_product,
  p.product_name, 
  p.category_number,
  p.characteristics
FROM 
  Product p
";
            var command = new MySqlCommand(query, _connection);
            using (var reader = await command.ExecuteReaderAsync())
            {
                var list = new List<Product>();
                while (await reader.ReadAsync())
                {
                    var product = new Product
                    {
                        ProductId = reader.GetInt32(0),
                        ProductName = reader.GetString(1),
                        CategoryNumber = reader.GetInt32(2),
                        ProductCharacteristics = reader.GetString(3),
                    };
                    list.Add(product);
                }
                return list;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Db.GetProducts error");
            return new List<Product>();
        }
    }

    public async Task<int?> AddProduct(Product product)
    {
        try
        {
            var query =
@"INSERT INTO Product (product_name, category_number, characteristics )
VALUES ( @product_name, @category_number, @characteristics );
SELECT LAST_INSERT_ID();";
            var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@product_name", product.ProductName);
            command.Parameters.AddWithValue("@category_number", product.CategoryNumber);
            command.Parameters.AddWithValue("@characteristics", product.ProductCharacteristics);
            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Db.AddProduct error");
            return null;
        }
    }

    public async Task<bool> UpdateProduct(Product product)
    {
        try
        {
            var query =
@"UPDATE Product
SET product_name = @product_name,
    category_number = @category_number,
    characteristics = @characteristics
WHERE id_product = @id_product";
            var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@id_product", product.ProductId);
            command.Parameters.AddWithValue("@product_name", product.ProductName);
            command.Parameters.AddWithValue("@category_number", product.CategoryNumber);
            command.Parameters.AddWithValue("@characteristics", product.ProductCharacteristics);
            await command.ExecuteScalarAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Db.UpdateProduct error");
            return false;
        }
    }

    public async Task<bool> DeleteProduct(int id)
    {
        try
        {
            var query =
@"DELETE FROM Product
WHERE id_product = @id_product";
            var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@id_product", id);
            await command.ExecuteScalarAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Db.DeleteProduct error");
            return false;
        }
    }
}

