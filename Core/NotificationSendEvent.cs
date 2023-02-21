namespace Core;

public class NotificationSendEvent
{
    public const string EventName = "notification_send";
    
    public string Content { get; set; }

    public string From { get; set; }

    public string To { get; set; }

    public string Type { get; set; }
    
    public Guid ProviderId { get; set; }
}