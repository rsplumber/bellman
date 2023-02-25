using Core;
using Core.NotificationManagements;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sms.Fake;

public static class ServiceCollectionExtension
{
    public static void AddFakeSms(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<FakeEventHandler>();
        services.AddScoped<AbstractNotificationManagement, FakeNotificationManagement>();
    }
}