using System.Data;
using Npgsql;
using ShopApi.Enums;
using ShopApi.Models;

namespace ShopApi.Data;

public class Context
{
    private readonly string _connectionString;

    public Context(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("ConnectionString");
    }

    public IDbConnection CreateConnection()
    {
        var mapper = NpgsqlConnection.GlobalTypeMapper;
        // mapper.MapEnum<Status>("status");
        mapper.MapComposite<OrderNote>("product_note");
        
        return new NpgsqlConnection(_connectionString);
    }
}
