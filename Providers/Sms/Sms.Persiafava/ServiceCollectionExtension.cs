using System.Net.Http.Headers;
using Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sms.Persiafava;

public static class ServiceCollectionExtension
{
    public static void AddPersiafava(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddProvider<SendNotificationManagement>();
        services.AddHttpClient("persiafava", c =>
        {
            c.BaseAddress = new Uri("http://sms.persiafava.com");
            c.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
        });
    }
}