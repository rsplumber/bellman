using Core.Events;
using Core.Notifications;
using Core.Notifications.Types;
using Core.Providers.Types;
using DotNetCore.CAP;

namespace Core.NotificationManagements;

public abstract class AbstractNotificationManagement
{
    private readonly ICapPublisher _capPublisher;
    private readonly INotificationRepository _notificationRepository;

    public AbstractNotificationManagement(ICapPublisher capPublisher, INotificationRepository notificationRepository)
    {
        _capPublisher = capPublisher;
        _notificationRepository = notificationRepository;
    }

    public abstract string ProviderName { get; }

    public abstract string ProviderType { get; }

    protected abstract int MaximumRetryCount { get; }

    public ProviderStatus Status { get; set; } = ProviderStatus.Enable;

    protected abstract Task<bool> SendNotificationAsync(string content, string to, CancellationToken cancellationToken = default);

    protected virtual Task<bool> SendBatchNotificationAsync(string content, string[] to, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task SendAsync(SendNotificationRequest req, CancellationToken cancellationToken = default)
    {
        var notification = await InsertNotification(req, cancellationToken);

        if (MaximumRetryReached(notification))
        {
            await RaiseFailedEventAsync(notification, cancellationToken);
            return;
        }

        var success = await SendNotificationAsync(req.Content, req.To, cancellationToken);
        if (!success)
        {
            await RaiseSendEventAsync(req, cancellationToken);
            return;
        }

        await RaiseSentEventAsync(req.Id, cancellationToken);
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

        notification.Retry += 1;
        await _notificationRepository.UpdateAsync(notification, cancellationToken);
        return notification;
    }

    private async Task RaiseSendEventAsync(SendNotificationRequest req, CancellationToken cancellationToken = default)
    {
        await _capPublisher.PublishAsync(SendNotificationEvent.EventName + "_" + ProviderName, new SendNotificationEvent
        {
            RequestId = req.Id,
            Content = req.Content,
            To = req.To,
            From = ProviderName,
            Type = req.Type
        }, cancellationToken: cancellationToken);
    }

    private async Task RaiseFailedEventAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await _capPublisher.PublishAsync(NotificationFailedEvent.EventName, new NotificationFailedEvent
        {
            Id = notification.Id
        }, cancellationToken: cancellationToken);
    }

    private async Task RaiseSentEventAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _capPublisher.PublishAsync(NotificationSentEvent.EventName, new NotificationSentEvent
        {
            Id = id,
        }, cancellationToken: cancellationToken);
    }
}