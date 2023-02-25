using Core.Notifications;
using Core.Notifications.Types;
using DotNetCore.CAP;

namespace Core.Events;

internal sealed  class NotificationSentEventHandler : ICapSubscribe
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationSentEventHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    [CapSubscribe(NotificationSentEvent.EventName)]
    public async Task HandleAsync(NotificationSentEvent message, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.FindAsync(message.Id, cancellationToken);
        if (notification is null)
        {
            return;
        }
        notification.Status = NotificationStatus.Sent;
        await _notificationRepository.UpdateAsync(notification, cancellationToken);
    }
}