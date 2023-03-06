namespace Core.Notifications;

public sealed record SendNotificationRequest(Guid Id)
{
    public string Content { get; init; } = default!;

    public string[] To { get; init; } = Array.Empty<string>();
}