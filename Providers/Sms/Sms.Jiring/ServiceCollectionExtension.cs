using System.Net.Http.Headers;
using Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sms.Jiring;

public static class ServiceCollectionExtension
{
    public static void AddJiring(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddProvider<SendNotificationManagement>();
        services.AddHttpClient("jiring", c =>
        {
            c.BaseAddress = new Uri("https://sms.jiring.ir:8085/");
            c.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });
    }
}