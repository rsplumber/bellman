using Core;
using Core.Notifications.Services;
using Data.Sql;
using Savorboard.CAP.InMemoryMessageQueue;
using Sms.Magfa;

namespace Application;

internal static class ServiceCollectionExtension
{
    public static void AddNotificationService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddData(configuration);
        services.AddScoped<INotificationService, NotificationService>();

        services.AddCap(x =>
        {
            x.UseInMemoryMessageQueue();
            x.UseInMemoryStorage();
            x.UseDashboard();
        });

        services.AddCore(configuration);
        services.AddMagfa(configuration);
    }
}