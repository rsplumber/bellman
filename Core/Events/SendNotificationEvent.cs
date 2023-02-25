namespace Core.Events;

public class SendNotificationEvent
{
    public const string EventName = "notification_send";

    public Guid RequestId { get; set; } = Guid.NewGuid();

    public string Content { get; set; }

    public string From { get; set; }

    public string To { get; set; }

    public string Type { get; set; }
}