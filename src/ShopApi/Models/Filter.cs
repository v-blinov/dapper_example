using ShopApi.Enums;

namespace ShopApi.Models;

public record Filter
{
    public IEnumerable<long>? StorageIds { get; init; } = null;
    public IEnumerable<Status>? Status { get; init; } = null;
    public Period? OrderPeriod { get; init; } = null;
    public Period? ReceivingPeriod { get; init; } = null;
}
