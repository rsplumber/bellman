using Core.Domains.Pattern;
using Core.Domains.Pattern.Exceptions;
using Core.Events;
using Core.Events.Pattern;
using Core.Providers;
using Core.Providers.Exceptions;
using Core.Providers.Types;
using DotNetCore.CAP;

namespace Core.Notifications;

public sealed record SendNotificationWithContent
{
    public string Content { get; init; } = default!;

    public string[] To { get; init; } = default!;

    public string Type { get; init; } = default!;

    public string? Provider { get; set; }
}

public sealed record SendNotificationWithPatternId
{
    public string[] Parameters { get; init; } = default!;

    public Guid PatternId { get; init; } = default!;

    public string To { get; init; } = default!;

    public string Type { get; init; } = default!;

    public string? Provider { get; init; }
}

public sealed record DeliveryStatusRequest
{
    public string PhoneNumber { get; init; } = default!;

    public Guid Id { get; init; } = default!;
}

public interface INotificationService
{
    Task SendAsync(SendNotificationWithContent request, CancellationToken cancellationToken = default);
    Task<SendNotificationResponse> SendAsync(SendNotificationWithPatternId request, CancellationToken cancellationToken = default);
    Task<DeliveryStatusNotificationResponse?> DeliveryStatusAsync(DeliveryStatusRequest request, CancellationToken cancellationToken = default);
}

internal sealed class NotificationService : INotificationService
{
    private readonly ICapPublisher _eventBus;
    private readonly IProviderSelectionService _providerSelectionService;
    private readonly IEnumerable<AbstractNotificationPatternManagement> _notificationManagements;
    private readonly INotificationRepository _notificationRepository;
    private readonly IPatternRepository _patternRepository;

    public NotificationService(IProviderSelectionService providerSelectionService, ICapPublisher eventBus, IEnumerable<AbstractNotificationPatternManagement> notificationManagements, INotificationRepository notificationRepository, IPatternRepository patternRepository)
    {
        _providerSelectionService = providerSelectionService;
        _eventBus = eventBus;
        _notificationManagements = notificationManagements;
        _notificationRepository = notificationRepository;
        _patternRepository = patternRepository;
    }

    public async Task SendAsync(SendNotificationWithContent request, CancellationToken cancellationToken = default)
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

        if (provider is null) throw new ProviderNotFoundException();

        if (provider.Status is not ProviderStatus.Enable) throw new ProviderDisabledException();

        await _eventBus.PublishAsync($"{SendNotificationEvent.EventName}.{provider.Type}.persiafava", new SendNotificationPatternEvent()
        {
            Content = request.Content,
            To = request.To.FirstOrDefault() ?? string.Empty,
            Provider = provider.Name
        }, cancellationToken: cancellationToken);
    }

    public async Task<SendNotificationResponse> SendAsync(SendNotificationWithPatternId request, CancellationToken cancellationToken = default)
    {
        var pattern = await _patternRepository.FindAsync(request.PatternId, cancellationToken);
        if (pattern is null) throw new PatternNotFoundException();
        if (pattern?.Parameters?.Count != request.Parameters.Length) throw new PatternInvalidParametersException();

        Provider? provider;
        if (request.Provider is not null)
        {
            provider = await _providerSelectionService.ResolveByNameAsync(request.Provider, cancellationToken);
        }
        else
        {
            provider = await _providerSelectionService.ResolveByTypeAsync(request.Type, cancellationToken);
        }

        if (provider is null) throw new ProviderNotFoundException();

        if (provider.Status is not ProviderStatus.Enable) throw new ProviderDisabledException();

        var message = new SendNotificationPatternEvent
        {
            PatternId = request.PatternId,
            Parameters = request.Parameters,
            To = request.To,
            Provider = provider.Name
        };
        await _eventBus.PublishAsync($"{SendNotificationPatternEvent.EventName}.{provider.Type}.{provider.Name}", message, cancellationToken: cancellationToken);

        var response = new SendNotificationResponse()
        {
            Data = new SendNotificationResponse.SendNotificationResponseModel()
            {
                Id = message.RequestId,
                PhoneNumber = message.To
            }
        };
        return response;
    }

    public async Task<DeliveryStatusNotificationResponse?> DeliveryStatusAsync(DeliveryStatusRequest request, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.FindAsync(request.Id, cancellationToken);
        if (notification == null) return null;

        var provider = await _providerSelectionService.ResolveByNameAsync(notification.From, cancellationToken);
        //if (provider == null) return null;
        if (provider is null) throw new ProviderNotFoundException();

        var notificationManagement = _notificationManagements.First(p => p.ProviderName == provider.Name);

        var delivery = await notificationManagement.GetDeliveryAsync(new DeliveryStatusNotificationRequest()
        {
            NotificationId = request.Id
        }, cancellationToken);
        return new DeliveryStatusNotificationResponse()
        {
            Date = new DeliveryStatusNotificationResponse.DeliveryStatusNotificationResponseModel()
            {
                Status = delivery?.Status
            }
        };
    }
}