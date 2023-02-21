using Microsoft.EntityFrameworkCore;
using Quries.Providres;

namespace Data.Sql.Providers;

internal sealed class ProviderListQuery : IProviderListQuery
{
    private readonly NotificationCenterDbContext _dbContext;

    public ProviderListQuery(NotificationCenterDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ProviderResponse>> QueryAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Providers
            .Select(provider => new ProviderResponse
            {
                Id = provider.Id,
                Name = provider.Name,
                Type = provider.Type,
                Status = provider.Status.ToString(),
                Metas = provider.Metas
            })
            .ToListAsync(cancellationToken: cancellationToken);
    }
}