using Core.SendingNotifications.Types;

namespace Core.SendingNotifications.Services;

public sealed record CreateNotificationRequest
{
    public string Content { get; set; }

    public string From { get; set; }

    public string To { get; set; }

    public string ProviderType { get; set; }

    public Guid ProviderId { get; set; }

    public NotificationStatus Status { get; set; } = NotificationStatus.Sending;
}