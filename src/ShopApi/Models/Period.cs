namespace ShopApi.Models;

/// <summary>
/// DateTime in utc
/// </summary>
public record Period
{
    public DateTime? Start { get; init; }
    public DateTime? End { get; init; }
}
