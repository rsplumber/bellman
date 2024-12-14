using Core.Domains.Jirings;
using Core.Domains.Jirings.Notification;
using Microsoft.EntityFrameworkCore;

namespace Data.Jirings;

public class JiringNotificationRepository : IJiringNotificationRepository
{
    private readonly ApplicationDbContext _dbContext;

    public JiringNotificationRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(JiringNotification entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.JiringNotifications.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(JiringNotification entity, CancellationToken cancellationToken = default)
    {
        _dbContext.JiringNotifications.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(JiringNotification entity, CancellationToken cancellationToken = default)
    {
        _dbContext.JiringNotifications.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<JiringNotification?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.JiringNotifications
            .FirstOrDefaultAsync(jiring => jiring.Id == id, cancellationToken: cancellationToken);
    }

    public async Task<JiringNotification?> FindByNotificationIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.JiringNotifications
            .FirstOrDefaultAsync(jiring => jiring.Notification.Id == id, cancellationToken: cancellationToken);
    }
}