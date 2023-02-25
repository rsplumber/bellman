using Core.Notifications.Types;

namespace Core.Notifications;

public class Notification
{
    public Guid Id { get; set; }

    public string Content { get; set; }

    public string From { get; set; }

    public string To { get; set; }

    public string Type { get; set; }

    public int Retry { get; set; }
    
    public NotificationStatus Status { get; set; }

    public DateTime CreatedDateUtc { get; internal set; } = DateTime.UtcNow;
}