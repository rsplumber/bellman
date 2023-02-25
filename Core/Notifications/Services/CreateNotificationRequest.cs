using Core.Notifications.Types;

namespace Core.Notifications.Services;

public sealed record CreateNotificationRequest
{
    public required string Content { get; set; }

    public string From { get; set; }

    public required string To { get; set; }

    public string ProviderType { get; set; }

    public NotificationStatus Status { get; set; } = NotificationStatus.Sending;
}