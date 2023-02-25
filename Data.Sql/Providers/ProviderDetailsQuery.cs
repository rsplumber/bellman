using Core;
using Core.NotificationManagements;
using Core.Providers.Exceptions;
using Queries.Providers;

namespace Data.Sql.Providers;

internal sealed class ProviderDetailsQuery : IProviderDetailsQuery
{
    private readonly IEnumerable<AbstractNotificationManagement> _notificationManagement;


    public ProviderDetailsQuery(IEnumerable<AbstractNotificationManagement> notificationManagement)
    {
        _notificationManagement = notificationManagement;
    }

    public Task<ProviderResponse> QueryAsync(string name, CancellationToken cancellationToken = default)
    {
        var provider = _notificationManagement
            .FirstOrDefault(model => model.ProviderName == name);

        if (provider is null)
        {
            throw new ProviderNotFoundException();
        }


        return Task.FromResult(new ProviderResponse
        {
            Name = provider.ProviderName,
            Type = provider.ProviderType,
            Status = provider.Status.ToString(),
        });
    }
}