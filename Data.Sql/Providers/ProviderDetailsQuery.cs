using Core.Providers;
using Core.Providers.Exceptions;
using Queries.Providers;

namespace Data.Sql.Providers;

internal sealed class ProviderDetailsQuery : IProviderDetailsQuery
{
    private readonly IProviderCollection _providerCollection;

    public ProviderDetailsQuery(IProviderCollection providerCollection)
    {
        _providerCollection = providerCollection;
    }

    public Task<ProviderResponse> QueryAsync(string name, CancellationToken cancellationToken = default)
    {
        var provider = _providerCollection.Providers
            .FirstOrDefault(model => model.Name == name);

        if (provider is null)
        {
            throw new ProviderNotFoundException();
        }


        return Task.FromResult(new ProviderResponse
        {
            Name = provider.Name,
            Type = provider.Type,
            Status = provider.Status.ToString(),
            Metas = provider.Metas,
        });
    }
}