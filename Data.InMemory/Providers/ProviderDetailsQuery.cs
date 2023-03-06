using Core.Providers;
using Core.Providers.Exceptions;
using Queries.Providers;

namespace Data.InMemory.Providers;

internal sealed class ProviderDetailsQuery : IProviderDetailsQuery
{
    private readonly IProviderRepository _providerRepository;

    public ProviderDetailsQuery(IProviderRepository providerRepository)
    {
        _providerRepository = providerRepository;
    }

    public async Task<ProviderResponse> QueryAsync(string name, CancellationToken cancellationToken = default)
    {
        var provider = await _providerRepository.FindByNameAsync(name, cancellationToken);
        if (provider is null)
        {
            throw new ProviderNotFoundException();
        }

        return new ProviderResponse
        {
            Name = provider.Name,
            Type = provider.Type,
            Status = provider.Status.ToString(),
        };
    }
}