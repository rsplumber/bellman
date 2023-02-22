using Core;
using Core.Notifications.Services;
using Core.Providers;
using Data.Sql;
using Savorboard.CAP.InMemoryMessageQueue;
using Sms.Fake;

namespace Application;

internal static class ServiceCollectionExtension
{
    public static void AddNotificationService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddData(configuration);
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<INotificationManagement, NotificationManagement>();
        services.AddSingleton<IProviderCollection, ProviderCollection>();

        services.AddCap(x =>
        {
            x.UseInMemoryMessageQueue();
            x.UseInMemoryStorage();
            x.UseDashboard();
        });

        services.AddScoped<ProviderSelectionEventHandler>();
        services.AddScoped<NotificationSendEventHandler>();
        services.AddFakeSms(configuration);
    }
}