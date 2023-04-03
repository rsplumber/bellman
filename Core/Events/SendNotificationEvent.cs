using Core.Notifications;
using DotNetCore.CAP;

namespace Core.Events;

public sealed record SendNotificationEvent
{
    public const string EventName = "bellman.notification.send";

    public Guid RequestId { get; set; } = Guid.NewGuid();

    public string Content { get; init; } = default!;

    public string[] To { get; init; } = Array.Empty<string>();

    public string Provider { get; init; } = default!;
}

internal sealed class SendNotificationEventHandler : ICapSubscribe
{
    private readonly IEnumerable<AbstractNotificationManagement> _notificationManagements;

    public SendNotificationEventHandler(IEnumerable<AbstractNotificationManagement> notificationManagement)
    {
        _notificationManagements = notificationManagement;
    }

    [CapSubscribe("bellman.notification.send.*")]
    public Task HandleAsync(SendNotificationEvent message)
    {
        var notificationManagement = _notificationManagements.First(p => p.ProviderName == message.Provider);
        return notificationManagement.SendAsync(new SendNotificationRequest(message.RequestId)
        {
            Content = message.Content,
            To = message.To
        });
    }
}