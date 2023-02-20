namespace Core.SendingNotifications.Services;

public sealed record DeleteSendingNotificationRequest
{
    public Guid Id { get; set; }
}