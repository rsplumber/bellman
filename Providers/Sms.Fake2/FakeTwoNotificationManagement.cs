using Core;
using Core.NotificationManagements;
using Core.Notifications;
using DotNetCore.CAP;

namespace Sms.Fake2;

public class FakeTwoNotificationManagement : AbstractNotificationManagement
{
    public override string ProviderName => "fake2";
    
    public override string ProviderType => "sms";
    
    protected override int MaximumRetryCount => 2;

    protected override async Task<bool> SendNotificationAsync(string content, string to, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("failed");
        return true;
    }

    public FakeTwoNotificationManagement(ICapPublisher capPublisher, INotificationRepository notificationRepository) : base(capPublisher, notificationRepository)
    {
    }
}