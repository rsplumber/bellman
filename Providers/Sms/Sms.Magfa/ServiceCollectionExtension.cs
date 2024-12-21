using System.Net.Http.Headers;
using Core;
using Core.Content;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sms.Magfa;

public static class ServiceCollectionExtension
{
    public static void AddMagfa(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddProviderPattern<SendNotificationManagement>();
        services.AddHttpClient("magfa", c =>
        {
            c.BaseAddress = new Uri("https://sms.magfa.com/api");
            c.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });
    }
}