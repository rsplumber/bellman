namespace Core.Notifications.Services;

public sealed record DeleteNotificationRequest
{
    public Guid Id { get; set; }
}