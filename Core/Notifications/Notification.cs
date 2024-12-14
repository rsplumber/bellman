using Core.Domains.Pattern;
using Core.Notifications.Types;

namespace Core.Notifications;

public class Notification
{
    public Guid Id { get; set; }

    public string Content { get; init; } = default!;
    
    public List<string>? Params { get; init; }
    
    public Pattern? Pattern { get; init; }

    public string From { get; init; } = default!;

    public List<string> To { get; init; } = new();

    public string Type { get; init; } = default!;

    public int Retry { get; private set; }

    public NotificationStatus Status { get; private set; } = NotificationStatus.Sending;
    
    public NotificationDeliveryStatus? DeliveryStatus { get; private set; } = NotificationDeliveryStatus.Unknown;

    public DateTime CreatedDateUtc { get; internal set; } = DateTime.UtcNow;
    

    public void IncrementRetry() => Retry += 1;

    public void Failed() => Status = NotificationStatus.Failed;

    public void Sent() => Status = NotificationStatus.Sent;

    public void SetDeliveryStatus(NotificationDeliveryStatus status) => DeliveryStatus = status;
    public void Delivered() => DeliveryStatus = NotificationDeliveryStatus.Delivered;
    public void UnDelivered() => DeliveryStatus = NotificationDeliveryStatus.UnDelivered;
}