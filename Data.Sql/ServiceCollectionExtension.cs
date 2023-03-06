using Core.Notifications;
using Data.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Queries.Notifications;

namespace Data;

public static class ServiceCollectionExtension
{
    public static void AddData(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(
            builder => builder.UseNpgsql(configuration.GetConnectionString("Default")));
        services.AddScoped<INotificationRepository, NotificationRepository>();

        services.AddScoped<INotificationDetailsQuery, NotificationDetailsQuery>();
        services.AddScoped<INotificationListQuery, NotificationListQuery>();
    }
}