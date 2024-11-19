using Core.Events;
using Core.Events.Pattern;
using Core.Notifications;
using Core.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Core.Content;

public static class ServiceCollectionPatternExtension
{
    public static void AddCorePattern(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddScoped<NotificationSentEventHandler>();
        services.TryAddScoped<NotificationFailedEventHandler>();
        services.TryAddScoped<SendNotificationPatternEventHandler>();
        services.TryAddScoped<INotificationService, NotificationService>();
        services.TryAddScoped<IProviderSelectionService, ProviderSelectionService>();
        services.TryAddScoped<IProviderService, ProviderService>();
    }

    public static void AddProviderPattern<TProvider>(this IServiceCollection services)
        where TProvider : AbstractNotificationPatternManagement
    {
        services.AddScoped<AbstractNotificationPatternManagement, TProvider>();
    }
}