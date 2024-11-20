using Core.Domains.Pattern;
using Core.Events;
using Core.Events.Pattern;
using Core.Notifications.Types;
using DotNetCore.CAP;

namespace Core.Notifications;

public abstract class AbstractNotificationPatternManagement
{
    private readonly ICapPublisher _eventBus;
    private readonly INotificationRepository _notificationRepository;
    private readonly IPatternRepository _patternRepository;

    protected AbstractNotificationPatternManagement(ICapPublisher eventBus, INotificationRepository notificationRepository, IPatternRepository patternRepository)
    {
        _eventBus = eventBus;
        _notificationRepository = notificationRepository;
        _patternRepository = patternRepository;
    }

    public abstract string ProviderName { get; }

    public abstract string ProviderType { get; }

    protected abstract int MaximumRetryCount { get; }


    protected abstract Task<SendNotification?> SendNotificationAsync(Notification notification, Guid? patternId, string[]? parameters, string to, string? content, CancellationToken cancellationToken);

    //protected abstract Task<SendNotification?> SendBatchNotificationAsync(Guid patternId, string[] parameters, string[] to, CancellationToken cancellationToken);

    protected abstract Task<GetDeliveryNotification?> GetDeliveryStatusNotificationAsync(Guid id, CancellationToken cancellationToken);

    public async Task SendAsync(SendNotificationWithPatternRequest req, CancellationToken cancellationToken = default)
    {
        var notification = await GetOrAddNotification(req, cancellationToken);

        if (MaximumRetryReached(notification))
        {
            await RaiseFailedEventAsync(notification, cancellationToken);
            return;
        }

        var notificationSent = await SendNotificationAsync(notification, req.PatternId, req.Parameters, req.To.First(), req.Content, cancellationToken);


        if (notificationSent is null)
        {
            await RaiseSendEventAsync(req, cancellationToken);
            return;
        }

        await _notificationRepository.UpdateAsync(notification, cancellationToken);

        await RaiseSentEventAsync(req.Id, cancellationToken);
    }

    public async Task<GetDeliveryNotification?> GetDeliveryAsync(DeliveryStatusNotificationRequest req, CancellationToken cancellationToken = default)
    {
        var delivery = await GetDeliveryStatusNotificationAsync(req.NotificationId, cancellationToken);
        if (delivery is null) return delivery;
        var notification = await _notificationRepository.FindAsync(req.NotificationId, cancellationToken);
        notification?.SetDeliveryStatus(delivery.Status);
        await _notificationRepository.UpdateAsync(notification, cancellationToken);
        return delivery;
    }


    private bool MaximumRetryReached(Notification notification)
    {
        return notification.Retry >= MaximumRetryCount;
    }

    private async Task<Notification> GetOrAddNotification(SendNotificationWithPatternRequest req, CancellationToken cancellationToken)
    {
        var notification = await _notificationRepository.FindAsync(req.Id, cancellationToken);
        Pattern? pattern = null;
        if (req.PatternId is not null)
        {
            pattern = await _patternRepository.FindAsync((Guid)req.PatternId, cancellationToken);
        }

        if (notification is null)
        {
            var createdNotification = new Notification
            {
                Id = req.Id,
                Type = ProviderType,
                From = ProviderName,
                Params = req.Parameters?.ToList(),
                Content = string.Format(pattern?.Template ?? string.Empty, req.Parameters?.Cast<object>().ToArray() ?? []),
                Pattern = pattern,
                To = req.To.ToList(),
            };
            await _notificationRepository.AddAsync(createdNotification, cancellationToken);
            return createdNotification;
        }

        notification.IncrementRetry();
        await _notificationRepository.UpdateAsync(notification, cancellationToken);
        return notification;
    }

    private async Task RaiseSendEventAsync(SendNotificationWithPatternRequest req, CancellationToken cancellationToken = default)
    {
        await _eventBus.PublishAsync($"{SendNotificationPatternEvent.EventName}.{ProviderType}.{ProviderName}", new SendNotificationPatternEvent()
        {
            RequestId = req.Id,
            Parameters = req.Parameters ?? [],
            PatternId = req.PatternId,
            To = req.To.First(),
            Provider = ProviderName
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

public class SendNotification
{
    public required string MessageId { get; init; }
}

public class GetDeliveryNotification
{
    public required NotificationDeliveryStatus Status { get; init; }
}