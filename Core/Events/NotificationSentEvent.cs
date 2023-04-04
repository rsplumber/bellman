using Core.Notifications;
using DotNetCore.CAP;

namespace Core.Events;

public sealed record NotificationSentEvent
{
    public const string EventName = "bellman.notification.sent";

    public Guid Id { get; init; }
}

internal sealed class NotificationSentEventHandler : ICapSubscribe
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationSentEventHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    [CapSubscribe(NotificationSentEvent.EventName, Group = "bellman.notification.sent")]
    public async Task HandleAsync(NotificationSentEvent message, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.FindAsync(message.Id, cancellationToken);
        if (notification is null)
        {
            return;
        }

        notification.Sent();
        await _notificationRepository.UpdateAsync(notification, cancellationToken);
    }
}