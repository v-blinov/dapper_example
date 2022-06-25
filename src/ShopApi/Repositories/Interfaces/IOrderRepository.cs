using ShopApi.Models;

namespace ShopApi.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetByFilter(Filter filter);
    Task<IEnumerable<Order>> GetWithStreamByFilter(Filter filter);
    Task CreateOrder(Order? order);
    Task CreateOrder(IEnumerable<Order> orders);
}
