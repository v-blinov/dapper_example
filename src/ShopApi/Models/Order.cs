using ShopApi.Enums;

namespace ShopApi.Models;

public record Order
{
    private readonly DateTime _creationDate;
    private readonly DateTime? _receivingDate;
    
    public Guid Id { get; init; } = Guid.NewGuid();
    public long ClientId { get; init; }
    public DateTime CreationDate
    {
        get => _creationDate;
        init => _creationDate = value.ToUniversalTime();
    }
    public DateTime? ReceivingDate
    {
        get => _receivingDate;
        init => _receivingDate = value?.ToUniversalTime();
    }
    public Status Status { get; init; }
    public OrderNote[] Products { get; init; } = null!;
    public long StorageId { get; init; }
}
