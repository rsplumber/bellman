using Core.Domains.Pattern;
using Core.Events;
using Core.Events.Pattern;
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


    protected abstract Task<SendNotification?> SendNotificationAsync(Guid patternId, string[] parameters, string to, CancellationToken cancellationToken);

    protected abstract Task<bool> SendBatchNotificationAsync(Guid patternId, string[] parameters, string[] to, CancellationToken cancellationToken);

    // protected abstract Task<bool> GetDeliveryStatusNotificationAsync(Guid patternId, string[] parameters, string to, CancellationToken cancellationToken);
    //
    // protected abstract Task<bool> GetBatchDeliveryStatusNotificationAsync(Guid patternId, string[] parameters, string[] to, CancellationToken cancellationToken);

    public async Task SendAsync(SendNotificationWithPatternRequest req, CancellationToken cancellationToken = default)
    {
        var notification = await GetOrAddNotification(req, cancellationToken);

        if (MaximumRetryReached(notification))
        {
            await RaiseFailedEventAsync(notification, cancellationToken);
            return;
        }

        SendNotification? notificationSent = null;
        if (IsBatchRequest())
        {
            //notificationSent = await SendBatchNotificationAsync(req.PatternId, req.Parameters, req.To, cancellationToken);
        }
        else
        {
            notificationSent = await SendNotificationAsync(req.PatternId, req.Parameters, req.To[0], cancellationToken);
        }

        if (notificationSent is null)
        {
            await RaiseSendEventAsync(req, cancellationToken);
            return;
        }

        //notification.
        await _notificationRepository.UpdateAsync(notification, cancellationToken);
        
        await RaiseSentEventAsync(req.Id, cancellationToken);

        bool IsBatchRequest() => req.To.Length > 1;
        
        
    }


    private bool MaximumRetryReached(Notification notification)
    {
        return notification.Retry >= MaximumRetryCount;
    }

    private async Task<Notification> GetOrAddNotification(SendNotificationWithPatternRequest req, CancellationToken cancellationToken)
    {
        var notification = await _notificationRepository.FindAsync(req.Id, cancellationToken);
        var pattern = await _patternRepository.FindAsync(req.PatternId, cancellationToken);
        if (notification is null)
        {
            var createdNotification = new Notification
            {
                Id = req.Id,
                Type = ProviderType,
                From = ProviderName,
                Params = req.Parameters.ToList(),
                Content = "",
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
            Parameters = req.Parameters,
            PatternId = req.PatternId,
            To = req.To,
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
    public required List<string> Date { get; init; }
}