using Core.NotificationManagements;
using Core.Notifications;
using DotNetCore.CAP;

namespace Email.Fake;

public class FakeNotificationManagement : AbstractNotificationManagement
{
    public override string ProviderName => "fake_email";
    public override string ProviderType => "email";
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