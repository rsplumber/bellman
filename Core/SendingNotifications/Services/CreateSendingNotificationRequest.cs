namespace Core.SendingNotifications.Services;

public sealed record CreateSendingNotificationRequest
{
    public string Content { get; set; }

    public string From { get; set; }

    public string To { get; set; }

    public string ProviderType { get; set; }

    public Guid ProviderId { get; set; }
}