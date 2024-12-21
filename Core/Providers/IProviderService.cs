using Core.Providers.Exceptions;
using Core.Providers.Types;

namespace Core.Providers;

public interface IProviderService
{
    Task ToggleAsync(string name, CancellationToken cancellationToken = default);
    Task ActivationAsync(string name, string type, CancellationToken cancellationToken = default);
}

internal sealed class ProviderService : IProviderService
{
    private readonly IProviderRepository _providerRepository;

    public ProviderService(IProviderRepository providerRepository)
    {
        _providerRepository = providerRepository;
    }

    public async Task ToggleAsync(string name, CancellationToken cancellationToken = default)
    {
        var provider = await _providerRepository.FindByNameAsync(name, cancellationToken);
        if (provider is null)
        {
            throw new ProviderNotFoundException();
        }

        provider.Status = provider.Status switch
        {
            ProviderStatus.Enable => ProviderStatus.Disable,
            ProviderStatus.Disable => ProviderStatus.Enable,
            _ => provider.Status
        };

        await _providerRepository.UpdateAsync(provider, cancellationToken);
    }

    public async Task ActivationAsync(string name, string type, CancellationToken cancellationToken = default)
    {
        var providers = await _providerRepository.FindAsync(cancellationToken);


        foreach (var provider in providers.Where(provider => provider.Type == type))
        {
            provider.Status = ProviderStatus.Disable;
            await _providerRepository.UpdateAsync(provider, cancellationToken);
        }


        var targetProvider = await _providerRepository.FindByNameAsync(name, cancellationToken);
        if (targetProvider != null)
        {
            targetProvider.Status = ProviderStatus.Enable;
            await _providerRepository.UpdateAsync(targetProvider, cancellationToken);
        }
    }
}