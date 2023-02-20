namespace Core.FailedNotifications.Services;

public interface IFailedNotificationService
{
    Task CreateAsync(CreateFailedNotificationRequest req, CancellationToken cancellationToken);
}