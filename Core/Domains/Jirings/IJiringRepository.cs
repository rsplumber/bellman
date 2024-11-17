namespace Core.Domains.Jirings;

public interface IJiringRepository
{
    Task AddAsync(Core.Domains.Jiring.Jiring entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(Core.Domains.Jiring.Jiring entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(Core.Domains.Jiring.Jiring entity, CancellationToken cancellationToken = default);

    Task<Core.Domains.Jiring.Jiring?> FindAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<Core.Domains.Jiring.Jiring?> FindByPatternIdAsync(Guid patternId, CancellationToken cancellationToken = default);
}