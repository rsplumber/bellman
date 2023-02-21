using Core.Notifications.Exceptions;
using Microsoft.EntityFrameworkCore;
using Queries.Notifications;

namespace Data.Sql.Notifications;

internal sealed class NotificationDetailsQuery : INotificationDetailsQuery
{
    private readonly NotificationCenterDbContext _dbContext;

    public NotificationDetailsQuery(NotificationCenterDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<NotificationResponse> QueryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await _dbContext.Notifications
            .FirstOrDefaultAsync(model => model.Id == id, cancellationToken);

        if (notification is null)
        {
            throw new NotificationNotFoundException();
        }


        return new()
        {
            Id = notification.Id,
            To = notification.To,
            From = notification.From,
            Type = notification.Type,
            Status = notification.Status.ToString(),
            Content = notification.Content,
            ProviderId = notification.ProviderId,
            CreatedDateUtc = notification.CreatedDateUtc
        };
    }
}