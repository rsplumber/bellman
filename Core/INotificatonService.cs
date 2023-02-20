namespace Core;

public interface INotificationService
{
    Task SendAsync(SendNotificationRequest req, CancellationToken cancellationToken = default);
}