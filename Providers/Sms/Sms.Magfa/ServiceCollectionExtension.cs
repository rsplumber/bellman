using System.Net.Http.Headers;
using Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sms.Magfa;

public static class ServiceCollectionExtension
{
    public static void AddMagfa(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddProvider<SendNotificationManagement>();
        services.AddHttpClient("magfa", c =>
        {
            c.BaseAddress = new Uri("https://sms.magfa.com/api");
            c.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });
    }
}