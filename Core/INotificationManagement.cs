namespace Core;

public interface INotificationManagement
{
    Task SendAsync(SendNotificationRequest req, CancellationToken cancellationToken = default);
}