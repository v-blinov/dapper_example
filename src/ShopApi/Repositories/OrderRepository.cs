using System.Data;
using Dapper;
using ShopApi.Data;
using ShopApi.Models;
using ShopApi.Repositories.Interfaces;
using Filter = ShopApi.Models.Filter;

namespace ShopApi.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly Context _context;

    public OrderRepository(Context context)
    {
        _context = context;
    }
    
    // реализовать через итератор
    public async Task<IEnumerable<Order>> GetByFilter(Filter filter)
    {
        var query = @"
            select id, client_id as ClientId, creation_date as CreationDate, receiving_date as ReceivingDate, status as Status, storage_id as StorageId 
            from orders
            where 1=1";
        
        // TODO не получается прочитать products ни в массив, ни в строку

        var (conditionQuery, parameters) = BuildConditions(filter);
        if(!string.IsNullOrWhiteSpace(conditionQuery))
            query += conditionQuery;

        using var connection = _context.CreateConnection();
        var orders = await connection.QueryAsync<Order>(query, parameters);

        var subquery = "select products as Products from orders";
        var prod = await connection.QueryAsync<(long, int)[]>(subquery);
        
        return orders.ToArray();
    }
    private static (string query, DynamicParameters parameters) BuildConditions(Filter filter)
    {
        var queryConditions = string.Empty;
        var parameters = new DynamicParameters();

        if(filter.StorageIds is not null && filter.StorageIds.Any())
        {
            queryConditions += " and storage_id any(@StorageIds)";
            parameters.Add("StorageIds", filter.StorageIds.ToArray(), DbType.Object);
        }

        if(filter.Status is not null)
        {
            queryConditions += " and status in @Status";
            parameters.Add("Status", filter.Status.Select(p => p.ToString()).ToArray(), DbType.Object);
        }

        if(filter.OrderPeriod is not null)
        {
            if(filter.OrderPeriod.Start.HasValue)
            {
                queryConditions += " and creation_date >= @Start";
                parameters.Add("Start", filter.OrderPeriod.Start.Value.ToUniversalTime(), DbType.DateTime);
            }

            if(filter.OrderPeriod.End.HasValue)
            {
                queryConditions += " and creation_date <= @End";
                parameters.Add("End", filter.OrderPeriod.End.Value.ToUniversalTime(), DbType.DateTime);
            }
        }

        if(filter.ReceivingPeriod is not null)
        {
            if(filter.ReceivingPeriod.Start.HasValue)
            {
                queryConditions += " and receiving_date >= @Start";
                parameters.Add("Start", filter.ReceivingPeriod.Start.Value.ToUniversalTime(), DbType.DateTime);
            }

            if(filter.ReceivingPeriod.End.HasValue)
            {
                queryConditions += " and receiving_date <= @End";
                parameters.Add("End", filter.ReceivingPeriod.End.Value.ToUniversalTime(), DbType.DateTime);
            }
        }

        return (queryConditions, parameters);
    }

    public async Task<IEnumerable<Order>> GetWithStreamByFilter(Filter filter)
    {
        var query = "select id, client_id as ClientId, creation_date as CreationDate, receiving_date as ReceivingDate, status as Status, storage_id as StorageId from orders";
        // TODO не получается прочитать products ни в массив, ни в строку

        using var connection = _context.CreateConnection();
        var reader = await connection.QueryMultipleAsync(query);
        
        var orders = await reader.ReadAsync<Order>(buffered: false).ConfigureAwait(false);
        return Stream(reader, orders);
    }
    private static IEnumerable<Order> Stream(SqlMapper.GridReader reader, IEnumerable<Order> orders)
    {
        using (reader)
        {
            foreach (var order in orders)
                yield return order;
        }     
    }


    public async Task CreateOrder(Order? order)
    {
        var query = @"insert into orders (id, client_id, creation_date, receiving_date, status, products, storage_id) 
                      values (@Id, @ClientId, @CreationDate, @ReceivingDate, @Status::status, @Products::product_note[], @StorageId)";
        
        var parameters = new DynamicParameters();
        parameters.Add("Id", order.Id, DbType.Guid);
        parameters.Add("ClientId", order.ClientId, DbType.Int64);
        parameters.Add("CreationDate", order.CreationDate.ToUniversalTime(), DbType.DateTime);
        parameters.Add("ReceivingDate", order.ReceivingDate?.ToUniversalTime(), DbType.DateTime);
        parameters.Add("Status", order.Status.ToString(), DbType.String);
        parameters.Add("Products", order.Products.Select(p => p.ToString()).ToArray(), DbType.Object);
        parameters.Add("StorageId", order.StorageId, DbType.Int64);

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, parameters);
    }

    
    // TODO: insert bulk
    public Task CreateOrder(IEnumerable<Order> orders) 
        => throw new NotImplementedException();
}
