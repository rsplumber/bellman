using Core.Domains.Jiring;
using Core.Domains.Jirings;
using Core.Domains.Pattern;
using Microsoft.EntityFrameworkCore;

namespace Data.Patterns;

public class PatternRepository : IPatternRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PatternRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Pattern entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Patterns.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Pattern entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Patterns.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Pattern entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Patterns.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Pattern?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Patterns
            .FirstOrDefaultAsync(jiring => jiring.Id == id, cancellationToken: cancellationToken);
    }
}