using Core.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class ServiceCollectionExtension
{
    public static void AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ProviderSelectionEventHandler>();
        services.AddScoped<NotificationSentEventHandler>();
        services.AddScoped<NotificationFailedEventHandler>();
    }
}