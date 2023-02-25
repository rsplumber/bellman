namespace Core;

public class NotificationFailedEvent
{
    public const string EventName = "notification_failed";

    public Guid Id { get; set; }
}