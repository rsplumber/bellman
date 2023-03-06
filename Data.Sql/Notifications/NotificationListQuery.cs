using Microsoft.EntityFrameworkCore;
using Queries.Notifications;

namespace Data.Notifications;

internal sealed class NotificationListQuery : INotificationListQuery
{
    private readonly ApplicationDbContext _dbContext;

    public NotificationListQuery(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<NotificationResponse>> QueryAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Notifications
            .AsNoTracking()
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