namespace Core;

public class NotificationSentEvent
{
    public const string EventName = "notification_sent";

    public Guid Id { get; set; }
    
}