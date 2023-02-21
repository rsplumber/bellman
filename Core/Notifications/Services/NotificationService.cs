using Core.Notifications.Exceptions;

namespace Core.Notifications.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task CreateAsync(CreateNotificationRequest req, CancellationToken cancellationToken)
    {
        var notification = new Notification
        {
            Id = new Guid(),
            Content = req.Content,
            From = req.From,
            To = req.To,
            Type = req.ProviderType,
            ProviderId = req.ProviderId,
            Status = req.Status
        };

        await _notificationRepository.AddAsync(notification, cancellationToken);
    }

    public async Task DeleteAsync(DeleteNotificationRequest req, CancellationToken cancellationToken = default)
    {
        var sendingNotification = await _notificationRepository.FindAsync(req.Id, cancellationToken);
        if (sendingNotification is null)
        {
            throw new SendingNotificationNotFoundException();
        }

        await _notificationRepository.DeleteAsync(sendingNotification, cancellationToken);
    }
}