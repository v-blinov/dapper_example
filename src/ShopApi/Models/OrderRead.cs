using ShopApi.Enums;

namespace ShopApi.Models;

public record OrderRead
{
    public Guid Id { get; init; }
    public long ClientId { get; init; }
    public DateTime CreationDate { get; init; }
    public DateTime? ReceivingDate { get; init; }
    public Status Status { get; init; }
    public object Products { get; init; }
    public long StorageId { get; init; }
}
