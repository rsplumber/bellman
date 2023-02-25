namespace Core;

public sealed record SendNotificationRequest
{
    public Guid Id { get; } = Guid.NewGuid();
    
    public string Content { get; init; }

    public string To { get; init; }

    public string Type { get; init; }
}