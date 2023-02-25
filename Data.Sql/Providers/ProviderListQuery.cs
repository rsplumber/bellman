using Core;
using Core.NotificationManagements;
using Queries.Providers;

namespace Data.Sql.Providers;

internal sealed class ProviderListQuery : IProviderListQuery
{
    private readonly IEnumerable<AbstractNotificationManagement> _notificationManagements;


    public ProviderListQuery(IEnumerable<AbstractNotificationManagement> notificationManagements)
    {
        _notificationManagements = notificationManagements;
    }

    public Task<List<ProviderResponse>> QueryAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_notificationManagements
            .Select(provider => new ProviderResponse
            {
                Name = provider.ProviderName,
                Type = provider.ProviderType,
                Status = provider.Status.ToString(),
            }).ToList());
    }
}