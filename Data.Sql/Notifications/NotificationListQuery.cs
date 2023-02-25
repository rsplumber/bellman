using Microsoft.EntityFrameworkCore;
using Queries.Notifications;

namespace Data.Sql.Notifications;

internal sealed class NotificationListQuery : INotificationListQuery
{
    private readonly NotificationCenterDbContext _dbContext;

    public NotificationListQuery(NotificationCenterDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<NotificationResponse>> QueryAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Notifications
            .Select(notification => new NotificationResponse
            {
                Id = notification.Id,
                To = notification.To,
                From = notification.From,
                Type = notification.Type,
                Status = notification.Status.ToString(),
                Content = notification.Content,
                Retry = notification.Retry,
                CreatedDateUtc = notification.CreatedDateUtc
            })
            .ToListAsync(cancellationToken: cancellationToken);
    }
}