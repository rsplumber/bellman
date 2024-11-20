namespace Core.Domains.Jirings.Notification;

public interface IJiringNotificationRepository
{
    Task AddAsync(JiringNotification entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(JiringNotification entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(JiringNotification entity, CancellationToken cancellationToken = default);

    Task<JiringNotification?> FindAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<JiringNotification?> FindByNotificationIdAsync(Guid id, CancellationToken cancellationToken = default);
    
}