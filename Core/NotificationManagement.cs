using DotNetCore.CAP;

namespace Core;

public class NotificationManagement : INotificationManagement
{
    private readonly ICapPublisher _capPublisher;

    public NotificationManagement(ICapPublisher capPublisher)
    {
        _capPublisher = capPublisher;
    }

    public async Task SendAsync(SendNotificationRequest req, CancellationToken cancellationToken = default)
    {
        await _capPublisher.PublishAsync(ProviderSelectionEvent.EventName, new ProviderSelectionEvent
        {
            Content = req.Content,
            Type = req.Type,
            To = req.To
            //todo Get "From"
        }, cancellationToken: cancellationToken);
    }
}