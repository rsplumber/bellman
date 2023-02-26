namespace Core.Events;

public class ProviderSelectionEvent
{
    public const string EventName = "provider_selection";
    
    public string Content { get; set; }
    
    public string[] To { get; set; }

    public string Type { get; set; }
}