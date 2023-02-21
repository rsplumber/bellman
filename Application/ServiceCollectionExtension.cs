using Core.Notifications.Services;
using Core.Providers.Services;
using Savorboard.CAP.InMemoryMessageQueue;

namespace Application;

internal static class ServiceCollectionExtension
{
    public static void AddNotificationService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IProviderService, ProviderService>();

        services.AddCap(x =>
        {
            x.UseInMemoryMessageQueue();
            x.UseInMemoryStorage();
            x.UseDashboard();
        });
    }
}