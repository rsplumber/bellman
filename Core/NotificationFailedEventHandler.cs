using Core.Notifications;
using Core.Notifications.Types;
using DotNetCore.CAP;

namespace Core;

public class NotificationFailedEventHandler : ICapSubscribe
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationFailedEventHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    [CapSubscribe(NotificationFailedEvent.EventName)]
    public async Task HandleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.FindAsync(id, cancellationToken);
        notification.Status = NotificationStatus.Failed;
        await _notificationRepository.UpdateAsync(notification, cancellationToken);
    }
}