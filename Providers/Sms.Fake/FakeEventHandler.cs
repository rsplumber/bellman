using Core;
using Core.Providers;
using DotNetCore.CAP;

namespace Sms.Fake;

internal sealed class FakeEventHandler : ICapSubscribe
{
    [CapSubscribe("notification_send_fake")]
    public async Task HandleAsync(ProviderSelectionEvent message)
    {
        // var provider = (await _providerRepository.FindAsync(message.Type)).FirstOrDefault();

        Console.WriteLine(message.Content);
    }
}