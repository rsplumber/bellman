using Core.Notifications;
using Core.Notifications.Types;
using DotNetCore.CAP;

namespace Core;

public abstract class NotificationManagement : INotificationManagement
{
    private readonly ICapPublisher _capPublisher;
    private readonly INotificationRepository _notificationRepository;

    public NotificationManagement(ICapPublisher capPublisher, INotificationRepository notificationRepository)
    {
        _capPublisher = capPublisher;
        _notificationRepository = notificationRepository;
    }

    public abstract string ProviderName { get; }

    public abstract string ProviderType { get; }

    protected abstract int MaximumRetryCount { get; }

    protected abstract Task<bool> SendNotificationAsync(string content, string to, CancellationToken cancellationToken = default);

    public async Task SendAsync(SendNotificationRequest req, CancellationToken cancellationToken = default)
    {
        var notification = await InsertNotification(req, cancellationToken);

        if (MaximumRetryReached(notification))
        {
            await RaisedFailedEventAsync(notification, cancellationToken);
            return;
        }

        var sendNotificationAsync = await SendNotificationAsync(req.Content, req.To, cancellationToken);
        if (!sendNotificationAsync)
        {
            await RaisedSendEventAsync(req, cancellationToken);
            return;
        }

        await RaisedSentEventAsync(req.Id, cancellationToken);
    }

    private bool MaximumRetryReached(Notification notification)
    {
        return notification.Retry >= MaximumRetryCount;
    }

    private async Task<Notification> InsertNotification(SendNotificationRequest req, CancellationToken cancellationToken)
    {
        var notification = await _notificationRepository.FindAsync(req.Id, cancellationToken);
        if (notification is null)
        {
            var createdNotification = new Notification()
            {
                Id = req.Id,
                Content = req.Content,
                Type = req.Type,
                To = req.To,
                Status = NotificationStatus.Sending,
                Retry = 0,
                From = ProviderName
            };
            await _notificationRepository.AddAsync(createdNotification, cancellationToken);
            return createdNotification;
        }

        notification.Retry = +1;
        await _notificationRepository.UpdateAsync(notification, cancellationToken);
        return notification;
    }

    private async Task RaisedSendEventAsync(SendNotificationRequest req, CancellationToken cancellationToken = default)
    {
        await _capPublisher.PublishAsync(NotificationSendEvent.EventName + ProviderName, new NotificationSendEvent
        {
            RequestId = req.Id,
            Content = req.Content,
            Type = req.Type,
            To = req.To,
            From = ProviderName
        }, cancellationToken: cancellationToken);
    }

    private async Task RaisedFailedEventAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await _capPublisher.PublishAsync(NotificationSendEvent.EventName + ProviderName, new NotificationSendEvent
        {
            RequestId = notification.Id,
            Content = notification.Content,
            Type = notification.Type,
            To = notification.To,
            From = ProviderName
        }, cancellationToken: cancellationToken);
    }

    private async Task RaisedSentEventAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _capPublisher.PublishAsync(NotificationSentEvent.EventName, new NotificationSentEvent
        {
            Id = id,
        }, cancellationToken: cancellationToken);
    }
}