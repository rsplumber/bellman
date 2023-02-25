using Core.Notifications;
using Core.Notifications.Types;
using DotNetCore.CAP;

namespace Core;

public class NotificationSentEventHandler : ICapSubscribe
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationSentEventHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    [CapSubscribe(NotificationSentEvent.EventName)]
    public async Task HandleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.FindAsync(id, cancellationToken);
        notification.Status = NotificationStatus.Sent;
        await _notificationRepository.UpdateAsync(notification, cancellationToken);
    }
}