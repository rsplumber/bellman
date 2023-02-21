namespace Core.Notifications.Exceptions;

public class NotificationNotFoundException : ApplicationException
{
    public NotificationNotFoundException() : base($"Notification Not found")
    {
    }
}