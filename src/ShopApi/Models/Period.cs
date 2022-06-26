namespace ShopApi.Models;

/// <summary>
/// DateTime in utc
/// </summary>
public record Period
{
    private readonly DateTime? _start;
    private readonly DateTime? _end;

    public DateTime? Start
    {
        get => _start;
        init => _start = value?.ToUniversalTime();
    }
    public DateTime? End 
    {
        get => _end;
        init => _end = value?.ToUniversalTime();
    }
}
