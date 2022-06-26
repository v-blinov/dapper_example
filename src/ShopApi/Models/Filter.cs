using ShopApi.Enums;

namespace ShopApi.Models;

public record Filter
{
    public long? StorageId { get; init; }
    public Status Status { get; init; } = Status.Unknown;
    public Period? OrderPeriod { get; init; }
    public Period? ReceivingPeriod { get; init; }
}
