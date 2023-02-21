using DotNetCore.CAP;

namespace Core;

public class NotificationSendEventHandler : ICapSubscribe
{
    private readonly ICapPublisher _capPublisher;

    public NotificationSendEventHandler(ICapPublisher capPublisher)
    {
        _capPublisher = capPublisher;
    }

    [CapSubscribe(NotificationSendEvent.EventName)]
    public async Task HandleAsync(NotificationSendEvent message)
    {
        await _capPublisher.PublishAsync(NotificationSendEvent.EventName, new NotificationSendEvent
        {
            Content = message.Content,
            To = message.To,
            Type = message.Type,
            ProviderId = message.ProviderId,
            From = message.From
        });
    }
}