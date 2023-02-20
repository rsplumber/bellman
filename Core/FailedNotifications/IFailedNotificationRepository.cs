namespace Core.FailedNotifications;

public interface IFailedNotificationRepository
{
    Task AddAsync(FailedNotification entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(FailedNotification entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(FailedNotification entity, CancellationToken cancellationToken = default);
    
    Task<FailedNotification?> FindAsync(Guid id, string identifier, CancellationToken cancellationToken = default);
}