using Core.NotificationManagements;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sms.Magfa;

public static class ServiceCollectionExtension
{
    public static void AddMagfa(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<SendSmsEventHandler>();
        services.AddScoped<AbstractNotificationManagement, SendNotificationManagement>();
        services.AddHttpClient("magfa", c => { c.BaseAddress = new Uri("https://sms.magfa.com/api/"); });
    }
}