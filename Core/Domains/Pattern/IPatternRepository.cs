namespace Core.Domains.Pattern;

public interface IPatternRepository
{
    Task AddAsync(Pattern entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(Pattern entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(Pattern entity, CancellationToken cancellationToken = default);

    Task<Pattern?> FindAsync(Guid id, CancellationToken cancellationToken = default);
}