using Core;
using Core.NotificationManagements;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sms.Fake2;

public static class ServiceCollectionExtension
{
    public static void AddFakeSms2(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<FakeEventHandler>();
        services.AddScoped<AbstractNotificationManagement, FakeTwoNotificationManagement>();
    }
}