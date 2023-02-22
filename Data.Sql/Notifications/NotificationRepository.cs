using Core.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Data.Sql.Notifications;

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationCenterDbContext _dbContext;

    public NotificationRepository(NotificationCenterDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Notification entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Notifications.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Notification entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Notifications.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Notification entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Notifications.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Notification?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Notifications
            .FirstOrDefaultAsync(notification => notification.Id == id, cancellationToken: cancellationToken);
    }
}