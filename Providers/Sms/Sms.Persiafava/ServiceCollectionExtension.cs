using Core.NotificationManagements;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sms.Persiafava;

public static class ServiceCollectionExtension
{
    public static void AddPersiafava(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<SendSmsEventHandler>();
        services.AddScoped<AbstractNotificationManagement, SendNotificationManagement>();
        services.AddHttpClient("persiafava", c => { c.BaseAddress = new Uri("http://sms.persiafava.com/"); });
    }
}