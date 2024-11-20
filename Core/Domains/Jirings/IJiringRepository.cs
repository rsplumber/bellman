namespace Core.Domains.Jirings;

public interface IJiringRepository
{
    Task AddAsync(Jiring entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(Jiring entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(Jiring entity, CancellationToken cancellationToken = default);

    Task<Jiring?> FindAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<Jiring?> FindByPatternIdAsync(Guid patternId, CancellationToken cancellationToken = default);
}