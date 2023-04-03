using Core.Notifications;
using DotNetCore.CAP;

namespace Core.Events;

public sealed record NotificationFailedEvent
{
    public const string EventName = "bellman.notification.failed";

    public Guid Id { get; init; }
}

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

        notification.Failed();
        await _notificationRepository.UpdateAsync(notification, cancellationToken);
    }
}