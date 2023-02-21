namespace Queries.Notifications;

public class NotificationResponse
{
    public Guid Id { get; set; }

    public string Content { get; set; }

    public string From { get; set; }

    public string To { get; set; }

    public string Type { get; set; }

    public Guid ProviderId { get; set; }

    public string Status { get; set; }

    public DateTime CreatedDateUtc { get; set; }
}