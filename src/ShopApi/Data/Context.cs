using System.Data;
using Npgsql;

namespace ShopApi.Data;

public class Context
{
    private readonly string _connectionString;

    public Context(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("ConnectionString");
    }

    public IDbConnection CreateConnection()
    => new NpgsqlConnection(_connectionString);
}
