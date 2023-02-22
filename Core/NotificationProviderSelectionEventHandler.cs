using Core.Providers;
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
        // var provider = (await _providerRepository.FindAsync(message.Type)).FirstOrDefault();
        var pro = new Provider()
        {
            Id = new Guid(),
            Type = "sms",
            Name = "fake",
        };
        // if (provider is null)
        // {
        //     throw new ProviderNotFoundException();
        // }

        await _capPublisher.PublishAsync(NotificationSendEvent.EventName + "_" + pro.Name, new NotificationSendEvent
        {
            Content = message.Content,
            To = message.To,
            Type = message.Type,
        });
    }
}