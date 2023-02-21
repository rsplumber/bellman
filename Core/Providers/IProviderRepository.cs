namespace Core.Providers;

public interface IProviderRepository
{
    Task AddAsync(Provider entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(Provider entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(Provider entity, CancellationToken cancellationToken = default);

    Task<Provider?> FindAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<Provider?>> FindAsync(string type, CancellationToken cancellationToken = default);
}