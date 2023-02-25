namespace Core.NotificationManagements;

public sealed record SendNotificationRequest(Guid Id)
{
    public string Content { get; init; }

    public string To { get; init; }

    public string Type { get; init; }
}