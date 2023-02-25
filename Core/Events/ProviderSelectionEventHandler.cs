using Core.NotificationManagements;
using DotNetCore.CAP;

namespace Core.Events;

internal sealed class ProviderSelectionEventHandler : ICapSubscribe
{
    private readonly IEnumerable<AbstractNotificationManagement> _notificationManagements;
    private readonly ICapPublisher _capPublisher;

    public ProviderSelectionEventHandler(ICapPublisher capPublisher, IEnumerable<AbstractNotificationManagement> notificationManagements)
    {
        _capPublisher = capPublisher;
        _notificationManagements = notificationManagements;
    }

    [CapSubscribe(ProviderSelectionEvent.EventName)]
    public async Task HandleAsync(ProviderSelectionEvent message)
    {
        var provider = _notificationManagements.FirstOrDefault(management => management.ProviderType == message.Type);
        if (provider is null)
        {
            return;
        }

        await _capPublisher.PublishAsync(SendNotificationEvent.EventName + "_" + provider.ProviderName, new SendNotificationEvent
        {
            Content = message.Content,
            To = message.To,
            Type = message.Type
        });
    }
}