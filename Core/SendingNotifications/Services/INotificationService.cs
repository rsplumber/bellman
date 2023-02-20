namespace Core.SendingNotifications.Services;

public interface INotificationService
{
    Task CreateAsync(CreateNotificationRequest req, CancellationToken cancellationToken);
    
    Task DeleteAsync(DeleteNotificationRequest req, CancellationToken cancellationToken = default);
}