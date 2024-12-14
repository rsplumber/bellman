namespace Core.Domains.Jirings.Notification;

public class JiringNotification
{
    public JiringNotification()
    {
        
    }
    
    
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Notifications.Notification Notification { get; set; }
    
    public string MessageId { get; set; }
}