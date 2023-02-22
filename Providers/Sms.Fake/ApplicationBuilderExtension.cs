using Core.Providers;
using Core.Providers.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sms.Fake;

public static class ApplicationBuilderExtension
{
    public static void UseFakeSms(this IApplicationBuilder app, IConfiguration configuration)
    {
        using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
        if (serviceScope == null) return;
        try
        {
            var repository = serviceScope.ServiceProvider.GetRequiredService<IProviderRepository>();
            repository.AddAsync(new Provider()
            {
                Name = "fake",
                Type = "sms",
                Status = ProviderStatus.Enable,

            }).Wait();
        }
        catch (Exception)
        {
            // ignored
        }
    }
}