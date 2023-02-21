namespace Core;

public class NotificationProviderSelectionEvent
{
    public const string EventName = "provider_selection";
    
    public string Content { get; set; }

    public string From { get; set; }

    public string To { get; set; }

    public string Type { get; set; }
}