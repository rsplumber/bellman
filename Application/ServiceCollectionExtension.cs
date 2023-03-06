using Sms.Magfa;
using Sms.Persiafava;
using Sms.Ssmss;

namespace Application;

internal static class ServiceCollectionExtension
{
    public static void AddNotificationService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMagfa(configuration);
        services.AddPersiafava(configuration);
        services.AddSsmss(configuration);
    }
}