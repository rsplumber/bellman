using Core.Notifications;
using DotNetCore.CAP;

namespace Core.Events;

public sealed record SendNotificationEvent
{
    public const string EventName = "bellman.notification.send";

    public Guid RequestId { get; set; } = Guid.NewGuid();

    public string? Content { get; init; }

    public Guid? PatternId { get; init; }

    public string[]? Parameters { get; init; }
    
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

    [CapSubscribe("bellman.notification.send.sms.*", Group = "bellman.notification.send.sms.queue")]
    public Task HandleSmsAsync(SendNotificationEvent message)
    {
        var notificationManagement = _notificationManagements.First(p => p.ProviderName == message.Provider);
        
        return notificationManagement.SendAsync(new SendNotificationRequest(message.RequestId)
        {
            Content = message.Content,
            To = message.To
        });
    }

    [CapSubscribe("bellman.notification.send.email.*", Group = "bellman.notification.send.email.queue")]
    public Task HandleEmailAsync(SendNotificationEvent message)
    {
        var notificationManagement = _notificationManagements.First(p => p.ProviderName == message.Provider);
        return notificationManagement.SendAsync(new SendNotificationRequest(message.RequestId)
        {
            Content = message.Content,
            To = message.To
        });
    }

    [CapSubscribe("bellman.notification.send.push.*", Group = "bellman.notification.send.push.queue")]
    public Task HandlePushNotificationAsync(SendNotificationEvent message)
    {
        var notificationManagement = _notificationManagements.First(p => p.ProviderName == message.Provider);
        return notificationManagement.SendAsync(new SendNotificationRequest(message.RequestId)
        {
            Content = message.Content,
            To = message.To
        });
    }
}