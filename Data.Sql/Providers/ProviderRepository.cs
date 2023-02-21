using Core.Providers;

namespace Data.Sql.Providers;

public class ProviderRepository : IProviderRepository
{
    public Task AddAsync(Provider entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Provider entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Provider entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Provider?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<Provider?>> FindAsync(string type, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}