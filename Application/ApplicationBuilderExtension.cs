using Core.Notifications;
using Core.Providers;
using Core.Providers.Exceptions;
using Core.Providers.Types;
using Data;
using Microsoft.EntityFrameworkCore;

namespace Application;

internal static class ApplicationBuilderExtension
{
    public static void UseNotificationCenter(this IApplicationBuilder app, IConfiguration configuration)
    {
        using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
        if (serviceScope == null) return;
        try
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        var providerRepository = serviceScope.ServiceProvider.GetRequiredService<IProviderRepository>();
        var notificationManagements = serviceScope.ServiceProvider.GetRequiredService<IEnumerable<AbstractNotificationPatternManagement>>();
        foreach (var management in notificationManagements)
        {
            var provider = providerRepository.FindByNameAsync(management.ProviderName).Result;
            if (provider is not null)
            {
                throw new ProviderNameExistsException(provider.Name);
            }

            providerRepository.AddAsync(new Provider
            {
                Name = management.ProviderName,
                Type = management.ProviderType,
                Status = ProviderStatus.Enable
            }).Wait();
        }
    }
}