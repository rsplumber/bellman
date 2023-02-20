namespace Core.SendingNotifications.Services;

public interface ISendingNotificationService
{
    Task CreateAsync(CreateSendingNotificationRequest req, CancellationToken cancellationToken);
    
    Task DeleteAsync(DeleteSendingNotificationRequest req, CancellationToken cancellationToken = default);
}