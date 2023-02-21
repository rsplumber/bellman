namespace Queries.Notifications;

public interface INotificationDetailsQuery
{
    Task<NotificationResponse> QueryAsync(Guid id, CancellationToken cancellationToken = default);
}