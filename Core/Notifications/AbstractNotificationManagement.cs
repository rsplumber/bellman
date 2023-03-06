using Core.Events;
using DotNetCore.CAP;

namespace Core.Notifications;

public abstract class AbstractNotificationManagement
{
    private readonly ICapPublisher _eventBus;
    private readonly INotificationRepository _notificationRepository;

    protected AbstractNotificationManagement(ICapPublisher eventBus, INotificationRepository notificationRepository)
    {
        _eventBus = eventBus;
        _notificationRepository = notificationRepository;
    }

    public abstract string ProviderName { get; }

    public abstract string ProviderType { get; }

    protected abstract int MaximumRetryCount { get; }

    protected abstract Task<bool> SendNotificationAsync(string content, string to, CancellationToken cancellationToken);

    protected abstract Task<bool> SendBatchNotificationAsync(string content, string[] to, CancellationToken cancellationToken);

    protected virtual void Validate(string content, string to)
    {
    }

    protected virtual void ValidateBatch(string content, string[] to)
    {
    }

    public async Task SendAsync(SendNotificationRequest req, CancellationToken cancellationToken = default)
    {
        var notification = await GetOrAddNotification(req, cancellationToken);

        if (MaximumRetryReached(notification))
        {
            await RaiseFailedEventAsync(notification, cancellationToken);
            return;
        }

        bool notificationSent;
        if (IsBatchRequest())
        {
            ValidateBatch(req.Content, req.To);
            notificationSent = await SendBatchNotificationAsync(req.Content, req.To, cancellationToken);
        }
        else
        {
            Validate(req.Content, req.To[0]);
            notificationSent = await SendNotificationAsync(req.Content, req.To[0], cancellationToken);
        }

        if (!notificationSent)
        {
            await RaiseSendEventAsync(req, cancellationToken);
            return;
        }

        await RaiseSentEventAsync(req.Id, cancellationToken);

        bool IsBatchRequest() => req.To.Length > 1;
    }


    private bool MaximumRetryReached(Notification notification)
    {
        return notification.Retry >= MaximumRetryCount;
    }

    private async Task<Notification> GetOrAddNotification(SendNotificationRequest req, CancellationToken cancellationToken)
    {
        var notification = await _notificationRepository.FindAsync(req.Id, cancellationToken);
        if (notification is null)
        {
            var createdNotification = new Notification
            {
                Id = req.Id,
                Type = ProviderType,
                From = ProviderName,
                Content = req.Content,
                To = req.To.ToList(),
            };
            await _notificationRepository.AddAsync(createdNotification, cancellationToken);
            return createdNotification;
        }

        notification.IncrementRetry();
        await _notificationRepository.UpdateAsync(notification, cancellationToken);
        return notification;
    }

    private async Task RaiseSendEventAsync(SendNotificationRequest req, CancellationToken cancellationToken = default)
    {
        await _eventBus.PublishAsync(SendNotificationEvent.EventName + "." + ProviderName, new SendNotificationEvent
        {
            RequestId = req.Id,
            Content = req.Content,
            To = req.To,
        }, cancellationToken: cancellationToken);
    }

    private async Task RaiseFailedEventAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await _eventBus.PublishAsync(NotificationFailedEvent.EventName, new NotificationFailedEvent
        {
            Id = notification.Id
        }, cancellationToken: cancellationToken);
    }

    private async Task RaiseSentEventAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _eventBus.PublishAsync(NotificationSentEvent.EventName, new NotificationSentEvent
        {
            Id = id
        }, cancellationToken: cancellationToken);
    }
}