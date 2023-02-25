using Core.Notifications;
using Core.Notifications.Types;
using DotNetCore.CAP;

namespace Core.Events;

internal sealed class NotificationFailedEventHandler : ICapSubscribe
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationFailedEventHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    [CapSubscribe(NotificationFailedEvent.EventName)]
    public async Task HandleAsync(NotificationFailedEvent message, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.FindAsync(message.Id, cancellationToken);
        if (notification is null)
        {
            return;
        }
        notification.Status = NotificationStatus.Failed;
        await _notificationRepository.UpdateAsync(notification, cancellationToken);
    }
}