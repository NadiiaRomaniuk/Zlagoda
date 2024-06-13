using MySqlConnector;
using Zlagoda.Server.Controllers;

namespace Zlagoda.Server.Database;

public partial class Db
{
    private readonly ILogger<Db> _logger;
    private readonly MySqlConnection _connection;
    private readonly IConfiguration _config;

    public Db(ILogger<Db> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
        _connection = new MySqlConnection(_config.GetConnectionString("Connection"));
        _connection.Open();
    }

    ~Db()
    {
        if(_connection != null)
            _ = _connection.CloseAsync();
    }
}
