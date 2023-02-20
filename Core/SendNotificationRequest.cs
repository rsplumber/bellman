namespace Core;

public record SendNotificationRequest()
{
    public string Content { get; set; }

    public string To { get; set; }

    public string ProviderType { get; set; }
}