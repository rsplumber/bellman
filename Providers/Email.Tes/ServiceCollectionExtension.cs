using Core;
using Core.Content;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Emai.Tes;

public static class ServiceCollectionExtension
{
    public static void AddTesEmail(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddFluentEmail("app@tes.ir")
            .AddSmtpSender("mail.tes.ir", 25, "card@tes.ir", "Card@123@");
        services.AddProviderPattern<SendNotificationManagement>();
    }
}