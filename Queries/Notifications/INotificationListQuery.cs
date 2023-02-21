namespace Queries.Notifications;

public interface INotificationListQuery
{
    Task<List<NotificationResponse>> QueryAsync(CancellationToken cancellationToken = default);
}