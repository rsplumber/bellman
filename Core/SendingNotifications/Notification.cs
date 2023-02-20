using Core.SendingNotifications.Types;

namespace Core.SendingNotifications;

public class Notification
{
    public Guid Id { get; set; }

    public string Content { get; set; }

    public string From { get; set; }

    public string To { get; set; }

    public string ProviderType { get; set; }

    public Guid ProviderId { get; set; }
    
    public NotificationStatus Status { get; set; }

    public DateTime CreatedDateUtc { get; } = DateTime.UtcNow;
}