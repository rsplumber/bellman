using Core.Notifications.Exceptions;
using Microsoft.EntityFrameworkCore;
using Queries.Notifications;

namespace Data.Notifications;

internal sealed class NotificationDetailsQuery : INotificationDetailsQuery
{
    private readonly ApplicationDbContext _dbContext;

    public NotificationDetailsQuery(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<NotificationResponse> QueryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await _dbContext.Notifications
            .AsNoTracking()
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
            Retry = notification.Retry,
            CreatedDateUtc = notification.CreatedDateUtc
        };
    }
}