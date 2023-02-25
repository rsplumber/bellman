using Core.Notifications;
using Data.Sql.Notifications;
using Data.Sql.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Queries.Notifications;
using Queries.Providers;

namespace Data.Sql;

public static class ServiceCollectionExtension
{
    public static void AddData(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NotificationCenterDbContext>(
            builder => builder.UseNpgsql(configuration.GetConnectionString("Default")));
        services.AddScoped<INotificationRepository, NotificationRepository>();

        //Queries
        services.AddScoped<IProviderDetailsQuery, ProviderDetailsQuery>();
        services.AddScoped<IProviderListQuery, ProviderListQuery>();
        services.AddScoped<INotificationDetailsQuery, NotificationDetailsQuery>();
        services.AddScoped<INotificationListQuery, NotificationListQuery>();
    }
}