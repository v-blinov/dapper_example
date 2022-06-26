using System.Text.Json.Serialization;

namespace ShopApi.Models;

public record OrderNote
{
    [JsonPropertyName("productId")]
    public long ProductId { get; init; }
    
    [JsonPropertyName("count")]
    public int Count { get; init; }

    public override string ToString() 
        => $"({ProductId},{Count})";
}
