namespace Core.Events;

public class SendBatchNotificationEvent
{
    public const string EventName = "notification_batch_send";

    public Guid RequestId { get; set; } = Guid.NewGuid();

    public string Content { get; set; }

    public string From { get; set; }

    public string[] To { get; set; }

    public string Type { get; set; }
}