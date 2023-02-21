namespace Core.Notifications.Exceptions;

public class SendingNotificationNotFoundException : ApplicationException
{
    public SendingNotificationNotFoundException() : base($"SendingNotification Not found")
    {
    }
}