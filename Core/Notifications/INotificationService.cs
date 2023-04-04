using Core.Events;
using Core.Providers;
using Core.Providers.Exceptions;
using Core.Providers.Types;
using DotNetCore.CAP;

namespace Core.Notifications;

public sealed record SendNotification
{
    public string Content { get; init; } = default!;

    public string[] To { get; init; } = default!;

    public string Type { get; init; } = default!;

    public string? Provider { get; init; }
}

public interface INotificationService
{
    Task SendAsync(SendNotification request, CancellationToken cancellationToken = default);
}

internal sealed class NotificationService : INotificationService
{
    private readonly ICapPublisher _eventBus;
    private readonly IProviderSelectionService _providerSelectionService;

    public NotificationService(IProviderSelectionService providerSelectionService, ICapPublisher eventBus)
    {
        _providerSelectionService = providerSelectionService;
        _eventBus = eventBus;
    }

    public async Task SendAsync(SendNotification request, CancellationToken cancellationToken = default)
    {
        Provider? provider;
        if (request.Provider is not null)
        {
            provider = await _providerSelectionService.ResolveByNameAsync(request.Provider, cancellationToken);
        }
        else
        {
            provider = await _providerSelectionService.ResolveByTypeAsync(request.Type, cancellationToken);
        }

        if (provider is null)
        {
            throw new ProviderNotFoundException();
        }

        if (provider.Status is not ProviderStatus.Enable)
        {
            throw new ProviderDisabledException();
        }

        await _eventBus.PublishAsync($"{SendNotificationEvent.EventName}.{provider.Type}.{provider.Name}", new SendNotificationEvent
        {
            Content = request.Content,
            To = request.To,
            Provider = provider.Name
        }, cancellationToken: cancellationToken);
    }
}