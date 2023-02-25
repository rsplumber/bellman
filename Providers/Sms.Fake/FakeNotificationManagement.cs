using Core;
using Core.NotificationManagements;
using Core.Notifications;
using DotNetCore.CAP;

namespace Sms.Fake;

public class FakeNotificationManagement : AbstractNotificationManagement
{
    public override string ProviderName => "fake";
    public override string ProviderType => "sms";
    protected override int MaximumRetryCount => 2;

    protected override async Task<bool> SendNotificationAsync(string content, string to, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("send");
        return true;
    }

    public FakeNotificationManagement(ICapPublisher capPublisher, INotificationRepository notificationRepository) : base(capPublisher, notificationRepository)
    {
    }
}