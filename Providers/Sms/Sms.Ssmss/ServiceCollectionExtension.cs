using Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sms.Ssmss;

public static class ServiceCollectionExtension
{
    public static void AddSsmss(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddProvider<SendNotificationManagement>();
        services.AddHttpClient("ssmss", c =>
        {
            c.BaseAddress = new Uri("http://ssmss.ir/webservice");
        });
    }
}