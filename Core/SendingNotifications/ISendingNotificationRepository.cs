namespace Core.SendingNotifications;

public interface ISendingNotificationRepository
{
    Task AddAsync(SendingNotification entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(SendingNotification entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(SendingNotification entity, CancellationToken cancellationToken = default);

    Task<SendingNotification?> FindAsync(Guid id, CancellationToken cancellationToken = default);
}