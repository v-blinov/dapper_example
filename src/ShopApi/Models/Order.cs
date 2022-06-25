using ShopApi.Enums;

namespace ShopApi.Models;

public record Order
{
    public Guid Id { get; init; } = new();
    public long ClientId { get; init; }
    public DateTime CreationDate { get; init; }
    public DateTime? ReceivingDate { get; init; }
    public Status Status { get; init; }
    public IEnumerable<OrderNote>? Products { get; init; }
    public long StorageId { get; init; }
}
