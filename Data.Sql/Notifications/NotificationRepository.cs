using Core.Notifications;

namespace Data.Sql.Notifications;

public class NotificationRepository : INotificationRepository
{
    public Task AddAsync(Notification entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Notification entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Notification entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Notification?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}