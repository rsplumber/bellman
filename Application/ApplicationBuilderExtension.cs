using Data.Sql;
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
            var context = serviceScope.ServiceProvider.GetRequiredService<NotificationCenterDbContext>();
            context.Database.Migrate();
        }
        catch (Exception)
        {
            // ignored
        }
    }
}