namespace Core.Notifications;

public sealed record SendNotificationWithPatternRequest(Guid Id)
{
    public Guid? PatternId { get; init; }

    public string[]? Parameters { get; init; } = [];
    
    
    public string? Content { get; init; } 

    public string[] To { get; init; } = [];
}