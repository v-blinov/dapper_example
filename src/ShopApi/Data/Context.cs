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
        mapper.MapComposite<OrderNote>("product_note");

        // use for bulkUpdate
        // mapper.MapEnum<Status>("status");
        // mapper.MapComposite<OrderWrite>("order_item");
        return new NpgsqlConnection(_connectionString);
    }
}
