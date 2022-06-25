namespace ShopApi.Models;

public record OrderNote
{
    public long ProductId { get; init; }
    public int Count { get; init; }

    public override string ToString() =>
        $"({ProductId},{Count})";
}
