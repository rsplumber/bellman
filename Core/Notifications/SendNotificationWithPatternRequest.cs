namespace Core.Notifications;

public sealed record SendNotificationWithPatternRequest(Guid Id)
{
    public Guid PatternId { get; init; } = default!;

    public string[] Parameters { get; init; } = [];

    public string[] To { get; init; } = [];
}