using Core.Events;
using Core.NotificationManagements;
using DotNetCore.CAP;

namespace Sms.Magfa;

internal sealed class SendSmsEventHandler : ICapSubscribe
{
    private readonly AbstractNotificationManagement _notificationManagement;

    public SendSmsEventHandler(AbstractNotificationManagement notificationManagement)
    {
        _notificationManagement = notificationManagement;
    }

    [CapSubscribe("notification_send_magfa")]
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