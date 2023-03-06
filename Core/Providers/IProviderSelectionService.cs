using Core.Providers.Exceptions;

namespace Core.Providers;

public interface IProviderSelectionService
{
    Task<Provider?> ResolveByTypeAsync(string type, CancellationToken cancellationToken = default);

    Task<Provider?> ResolveByNameAsync(string name, CancellationToken cancellationToken = default);
}

internal sealed class ProviderSelectionService : IProviderSelectionService
{
    private readonly IProviderRepository _providerRepository;

    public ProviderSelectionService(IProviderRepository providerRepository)
    {
        _providerRepository = providerRepository;
    }

    public async Task<Provider?> ResolveByTypeAsync(string type, CancellationToken cancellationToken = default)
    {
        var providers = await _providerRepository.FindAsync(cancellationToken);
        return providers.FirstOrDefault(provider => provider.Type == type);
    }

    public async Task<Provider?> ResolveByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _providerRepository.FindByNameAsync(name, cancellationToken);;
    }
}