using Core.Domains.Jiring;
using Core.Domains.Jirings;
using Microsoft.EntityFrameworkCore;

namespace Data.Jirings;

public class JiringRepository : IJiringRepository
{
    private readonly ApplicationDbContext _dbContext;

    public JiringRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Jiring entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Jirings.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Jiring entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Jirings.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Jiring entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Jirings.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Jiring?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Jirings
            .FirstOrDefaultAsync(jiring => jiring.Id == id, cancellationToken: cancellationToken);
    }

    public async Task<Jiring?> FindByPatternIdAsync(Guid patternId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Jirings
            .FirstOrDefaultAsync(jiring => jiring.PatternId == patternId, cancellationToken: cancellationToken);
    }
}