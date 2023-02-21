using Core.Providers;
using Core.Providers.Exceptions;
using DotNetCore.CAP;

namespace Core;

public class NotificationProviderSelectionEventHandler : ICapSubscribe
{
    private readonly IProviderRepository _providerRepository;
    private readonly ICapPublisher _capPublisher;

    public NotificationProviderSelectionEventHandler(IProviderRepository providerRepository, ICapPublisher capPublisher)
    {
        _providerRepository = providerRepository;
        _capPublisher = capPublisher;
    }

    [CapSubscribe(NotificationProviderSelectionEvent.EventName)]
    public async Task HandleAsync(NotificationProviderSelectionEvent message)
    {
        var provider = (await _providerRepository.FindAsync(message.Type)).FirstOrDefault();
        if (provider is null)
        {
            throw new ProviderNotFoundException();
        }

        await _capPublisher.PublishAsync(NotificationSendEvent.EventName, new NotificationSendEvent
        {
            Content = message.Content,
            To = message.To,
            Type = message.Type,
            ProviderId = provider.Id
        });
    }
}