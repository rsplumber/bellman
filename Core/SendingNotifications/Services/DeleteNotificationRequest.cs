namespace Core.SendingNotifications.Services;

public sealed record DeleteNotificationRequest
{
    public Guid Id { get; set; }
}