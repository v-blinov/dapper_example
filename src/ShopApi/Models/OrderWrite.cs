namespace ShopApi.Models;

public record OrderWrite
{
    public Guid Id { get; init; }
    public long ClientId { get; init; }
    public DateTime CreationDate { get; init; }
    public DateTime? ReceivingDate { get; init; }
    public string Status { get; init; } = null!;
    public OrderNote[] Products { get; init; } = null!;
    public long StorageId { get; init; }

    public OrderWrite(Order order)
    {
        Id = order.Id;
        ClientId = order.ClientId;
        CreationDate = order.CreationDate;
        ReceivingDate = order.ReceivingDate;
        Status = order.Status.ToString();
        Products = order.Products;
        StorageId = order.StorageId;
    }
}
