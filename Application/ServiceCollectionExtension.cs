using Core;
using Core.Notifications.Services;
using Data.Sql;
using Email.Fake;
using Savorboard.CAP.InMemoryMessageQueue;
using Sms.Fake;
using Sms.Fake2;

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
        services.AddFakeSms(configuration);
        services.AddFakeSms2(configuration);
        services.AddFakeEmail(configuration);
    }
}