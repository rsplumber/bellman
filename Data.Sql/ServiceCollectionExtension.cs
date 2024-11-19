using Core.Domains.Jirings;
using Core.Domains.Pattern;
using Core.Notifications;
using Data.Jirings;
using Data.Notifications;
using Data.Patterns;
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
        services.AddScoped<IPatternRepository, PatternRepository>();
        services.AddScoped<IJiringRepository, JiringRepository>();
        services.AddScoped<INotificationDetailsQuery, NotificationDetailsQuery>();
        services.AddScoped<INotificationListQuery, NotificationListQuery>();
    }
}