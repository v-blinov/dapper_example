using System.Data;
using Dapper;
using ShopApi.Data;
using ShopApi.Enums;
using ShopApi.Models;
using ShopApi.Repositories.Interfaces;

namespace ShopApi.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly Context _context;

    public OrderRepository(Context context)
    {
        _context = context;
    }
    
    public async IAsyncEnumerable<Order>? GetWithStreamByFilter(Filter filter)
    {
        var query = @"select id, client_id as ClientId, creation_date as CreationDate, receiving_date as ReceivingDate, status as Status, products as Products, storage_id as StorageId
                      from orders
                      where (@StorageConditionIsEmpty or storage_id = @StorageId)
                          and (@StatusConditionIsEmpty or status = @Status::status)
                          and (@CreationStartConditionIsEmpty or creation_date >= @CreationStart)
                          and (@CreationEndConditionIsEmpty or creation_date <= @CreationEnd)
                          and (@ReceivingStartConditionIsEmpty or creation_date >= @ReceivingStart)
                          and (@ReceivingEndConditionIsEmpty or creation_date <= @ReceivingEnd)";
        
        var parameters = new DynamicParameters();
        
        parameters.Add("StorageId", filter.StorageId, DbType.Object);
        parameters.Add("Status", filter.Status.ToString());
        parameters.Add("CreationStart", filter.OrderPeriod?.Start, DbType.DateTime);
        parameters.Add("CreationEnd", filter.OrderPeriod?.End, DbType.DateTime);
        parameters.Add("ReceivingStart", filter.ReceivingPeriod?.Start, DbType.DateTime);
        parameters.Add("ReceivingEnd", filter.ReceivingPeriod?.End, DbType.DateTime);
        
        // Чтобы не склеивать строки, "активирую" условия, только если соответствующий фильтр задан
        parameters.Add("StorageConditionIsEmpty", !filter.StorageId.HasValue, DbType.Boolean);
        parameters.Add("StatusConditionIsEmpty", filter.Status == Status.Unknown, DbType.Boolean);
        parameters.Add("CreationStartConditionIsEmpty", !filter.OrderPeriod?.Start.HasValue ?? true, DbType.Boolean);
        parameters.Add("CreationEndConditionIsEmpty", !filter.OrderPeriod?.End.HasValue ?? true, DbType.Boolean);
        parameters.Add("ReceivingStartConditionIsEmpty", !filter.ReceivingPeriod?.Start.HasValue ?? true, DbType.Boolean);
        parameters.Add("ReceivingEndConditionIsEmpty", !filter.ReceivingPeriod?.End.HasValue ?? true, DbType.Boolean);

        using var connection = _context.CreateConnection();
        var reader = await connection.ExecuteReaderAsync(query, parameters);

        while(reader.Read())
        {
            yield return new Order
            {
                Id = (Guid)reader[0],
                ClientId = (long)reader[1],
                CreationDate = (DateTime)reader[2],
                ReceivingDate = (DateTime)reader[3],
                Status = DefineStatus(reader[4].ToString()!),
                Products = (OrderNote[])reader[5],
                StorageId = (long)reader[6]
            };
        }
    }

    
    public async Task CreateOrder(Order order)
    {
        var query = @"insert into orders (id, client_id, creation_date, receiving_date, status, products, storage_id)
                      values (@Id, @ClientId, @CreationDate, @ReceivingDate, @Status::status, @Products::product_note[], @StorageId)";
        
        var parameters = new DynamicParameters();
        parameters.Add("Id", order.Id, DbType.Guid);
        parameters.Add("ClientId", order.ClientId, DbType.Int64);
        parameters.Add("CreationDate", order.CreationDate, DbType.DateTime);
        parameters.Add("ReceivingDate", order.ReceivingDate, DbType.DateTime);
        parameters.Add("Status", order.Status.ToString(), DbType.String);
        parameters.Add("Products", order.Products.Select(p => p.ToString()).ToArray(), DbType.Object);
        parameters.Add("StorageId", order.StorageId, DbType.Int64);

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, parameters);
    }

    // TODO: insert bulk
    public Task CreateOrder(IEnumerable<Order> orders) 
        => throw new NotImplementedException();


    public async Task<IEnumerable<Order>> Test()
    {
        var filter = new Filter
        {
            StorageId = null,
            Status = Status.New,
            OrderPeriod = new Period { Start = DateTime.Now.AddMonths(-1), End = DateTime.Now },
            ReceivingPeriod = new Period { Start = DateTime.Now.AddMonths(-1).ToUniversalTime(), End = DateTime.Now.ToUniversalTime() }
        };

        var query = @"select id, client_id as ClientId, creation_date as CreationDate, receiving_date as ReceivingDate, status as Status, products as Products, storage_id as StorageId
                      from orders 
                      where @StorageConditionIsEmpty or storage_id = @StorageId";
        
        var parameters = new DynamicParameters();
        parameters.Add("StorageId", filter.StorageId);
        parameters.Add("StorageConditionIsEmpty", !filter.StorageId.HasValue);
        
        using var connection = _context.CreateConnection();
        var reader = await connection.ExecuteReaderAsync(query, parameters);

        var orders = new List<Order>();
        while(reader.Read())
        {
            orders.Add(new Order
            {
                Id = (Guid)reader[0],
                ClientId = (long)reader[1],
                CreationDate = (DateTime)reader[2],
                ReceivingDate = (DateTime)reader[3],
                Status = DefineStatus(reader[4].ToString()),
                Products = (OrderNote[])reader[5],
                StorageId = (long)reader[6]
            });
        }
        return orders;
    }

    private static Status DefineStatus(string? value)
    {
        return value switch
        {
            nameof(Status.New) => Status.New,
            nameof(Status.Pending) => Status.Pending,
            nameof(Status.InProgress) => Status.InProgress,
            _ => Status.Unknown,
        };
    }
}
