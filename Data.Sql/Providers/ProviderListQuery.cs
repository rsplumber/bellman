using Core.Providers;
using Queries.Providers;

namespace Data.Sql.Providers;

internal sealed class ProviderListQuery : IProviderListQuery
{
    private readonly IProviderCollection _providerCollection;

    public ProviderListQuery(IProviderCollection providerCollection)
    {
        _providerCollection = providerCollection;
    }

    public Task<List<ProviderResponse>> QueryAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_providerCollection.Providers
            .Select(provider => new ProviderResponse
            {
                Name = provider.Name,
                Type = provider.Type,
                Status = provider.Status.ToString(),
                Metas = provider.Metas
            }).ToList());
    }
}