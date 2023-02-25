using Core.Events;
using Core.NotificationManagements;
using DotNetCore.CAP;

namespace Sms.Fake;

internal sealed class FakeEventHandler : ICapSubscribe
{
    private readonly AbstractNotificationManagement _notificationManagement;

    public FakeEventHandler(AbstractNotificationManagement notificationManagement)
    {
        _notificationManagement = notificationManagement;
    }

    [CapSubscribe("notification_send_fake")]
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