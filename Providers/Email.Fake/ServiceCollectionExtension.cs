using Core.NotificationManagements;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Email.Fake;

public static class ServiceCollectionExtension
{
    public static void AddFakeEmail(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<FakeEventHandler>();
        services.AddScoped<AbstractNotificationManagement, FakeNotificationManagement>();
    }
}