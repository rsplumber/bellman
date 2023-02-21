using Core.Providers.Exceptions;
using Microsoft.EntityFrameworkCore;
using Queries.Providres;

namespace Data.Sql.Providers;

internal sealed class ProviderDetailsQuery : IProviderDetailsQuery
{
    private readonly NotificationCenterDbContext _dbContext;

    public ProviderDetailsQuery(NotificationCenterDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProviderResponse> QueryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var provider = await _dbContext.Providers
            .FirstOrDefaultAsync(model => model.Id == id, cancellationToken);

        if (provider is null)
        {
            throw new ProviderNotFoundException();
        }


        return new()
        {
            Id = provider.Id,
            Name = provider.Name,
            Type = provider.Type,
            Status = provider.Status.ToString(),
            Metas = provider.Metas
        };
    }
}