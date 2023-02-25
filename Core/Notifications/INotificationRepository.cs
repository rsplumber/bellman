namespace Core.Notifications;

public interface INotificationRepository
{
    Task AddAsync(Notification entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(Notification entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(Notification entity, CancellationToken cancellationToken = default);

    Task<Notification?> FindAsync(Guid id, CancellationToken cancellationToken = default);
}