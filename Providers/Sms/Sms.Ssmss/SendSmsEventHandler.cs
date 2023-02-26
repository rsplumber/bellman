using Core.Events;
using Core.NotificationManagements;
using DotNetCore.CAP;

namespace Sms.Ssmss;

internal sealed class SendSmsEventHandler : ICapSubscribe
{
    private readonly AbstractNotificationManagement _notificationManagement;

    public SendSmsEventHandler(IEnumerable<AbstractNotificationManagement> notificationManagement)
    {
        _notificationManagement = notificationManagement.FirstOrDefault(p => p.ProviderName == "persiafava")
                                  ?? throw new ArgumentException("Register provider");
    }

    [CapSubscribe("notification_send_ssmss")]
    public async Task HandleAsync(SendNotificationEvent message)
    {
        await _notificationManagement.SendAsync(new SendNotificationRequest(message.RequestId)
        {
            Content = message.Content,
            To = message.To,
            Type = message.Type
        });
        Console.WriteLine(message.Content);
    }
}