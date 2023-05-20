using Core.Events;
using Core.Notifications;
using Core.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Core;

public static class ServiceCollectionExtension
{
    public static void AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddScoped<NotificationSentEventHandler>();
        services.TryAddScoped<NotificationFailedEventHandler>();
        services.TryAddScoped<SendNotificationEventHandler>();
        services.TryAddScoped<INotificationService, NotificationService>();
        services.TryAddScoped<IProviderSelectionService, ProviderSelectionService>();
        services.TryAddScoped<IProviderService, ProviderService>();
    }

    public static void AddProvider<TProvider>(this IServiceCollection services)
        where TProvider : AbstractNotificationManagement
    {
        services.AddScoped<AbstractNotificationManagement, TProvider>();
    }
}