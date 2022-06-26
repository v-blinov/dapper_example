using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Enums;
using ShopApi.Models;
using ShopApi.Repositories.Interfaces;

namespace ShopApi.Controllers;

[Route("order")]
public class OrderController : Controller
{
    private readonly ILogger<OrderController> _logger;
    private readonly IOrderRepository _orderRepository;

    public OrderController(ILogger<OrderController> logger, IOrderRepository orderRepository)
    {
        _logger = logger;
        _orderRepository = orderRepository;
    }

    [HttpGet("orders")]
    public IAsyncEnumerable<Order>? GetOrders([FromQuery] Filter filter)
    {
        try
        {
            return _orderRepository.GetWithStreamByFilter(filter);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "");
            return null;
        }
    }
    
    
    [HttpPost("create/")]
    public async Task<ActionResult<IEnumerable<Order>>> CreateOrder([FromForm] Order orderInput)
    {        
        try
        {
            var order = orderInput with { Products = Request.Form["Products"].ToArray().Select(p => JsonSerializer.Deserialize<OrderNote>(p)).ToArray() };
            
            await _orderRepository.CreateOrder(order);
            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "");
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpPost("create/random")]
    public async Task<ActionResult<IEnumerable<Order>>> CreateOrder()
    {        
        var random = new Random();

        try
        {
            const int dateRange = 30;
            var dateStart = DateTime.Now.AddDays(-dateRange);
            
            var order = new Order
            {
                Id = Guid.NewGuid(),
                ClientId = random.NextInt64(),
                Status = GetRandomStatus(random.Next(0, 99)),
                CreationDate = dateStart.AddDays(random.Next(dateRange / 2)),
                ReceivingDate = dateStart.AddDays((dateRange / 2) + random.Next(dateRange / 2)),
                Products = GenerateNewProducts(random).ToArray(),
                StorageId = random.Next(1, 20)
            };

            await _orderRepository.CreateOrder(order);
            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "");
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpPost("create/random/many")]
    public async Task<ActionResult<IEnumerable<Order>>> CreateRandomOrder([FromForm] int count = 10)
    {
        var random = new Random();

        try
        {
            const int dateRange = 30;
            var dateStart = DateTime.Now.AddDays(-dateRange);

            var orders = new Order[count];
            for(var i = 0; i < count; i++)
            {
                orders[i] = new Order
                {
                    Id = Guid.NewGuid(),
                    ClientId = random.NextInt64(),
                    Status = GetRandomStatus(random.Next(0, 99)),
                    CreationDate = dateStart.AddDays(random.Next(dateRange / 2)),
                    ReceivingDate = dateStart.AddDays((dateRange / 2) + random.Next(dateRange / 2)),
                    Products = GenerateNewProducts(random).ToArray(),
                    StorageId = random.Next(1, 20)
                };
            }

            await _orderRepository.CreateOrder(orders);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "");
            return StatusCode(500, ex.Message);
        }
    }
    
    
    [HttpGet("test")]
    public async Task<ActionResult<IEnumerable<Order>>> Test()
    {
        var orders = await _orderRepository.Test();
        return Ok(orders.ToArray());
    }
    
    
    private static IEnumerable<OrderNote> GenerateNewProducts(Random random)
    {
        var capacity = random.Next(1, 20);
        var instances = new OrderNote[capacity];
        for(var i = 0; i < capacity; i++)
        {
            instances[i] = new OrderNote
            {
                ProductId = random.NextInt64(),
                Count = random.Next(1,10)
            }; 
        }

        return instances;
    }
    private static Status GetRandomStatus(int rnd)
    {
        rnd %= 10;
        return rnd switch
        {
            <= 3 => Status.New,
            >= 7 => Status.Pending,
            _ => Status.InProgress
        };
    }
}
