using ShopApi.Models;

namespace ShopApi.Repositories.Interfaces;

public interface IOrderRepository
{
    IAsyncEnumerable<Order>? GetWithStreamByFilter(Filter filter);
    Task CreateOrder(Order order);
    Task CreateOrder(IEnumerable<Order> orders);
    Task<IEnumerable<Order>> Test();
}
