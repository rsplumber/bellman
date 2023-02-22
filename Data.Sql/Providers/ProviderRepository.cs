using Core.Providers;
using Microsoft.EntityFrameworkCore;

namespace Data.Sql.Providers;

public class ProviderRepository : IProviderRepository
{
    private readonly NotificationCenterDbContext _dbContext;

    public ProviderRepository(NotificationCenterDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Provider entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Providers.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Provider entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Providers.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Provider entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Providers.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Provider?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Providers
            .FirstOrDefaultAsync(provider => provider.Id == id, cancellationToken: cancellationToken);
    }

    public async Task<List<Provider>> FindAsync(string type, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Providers.Select(provider => provider)
            .Where(provider => provider.Type == type).ToListAsync(cancellationToken: cancellationToken);
    }
}