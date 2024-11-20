using Core.Notifications;
using DotNetCore.CAP;

namespace Core.Events.Pattern;

public sealed record SendNotificationPatternEvent
{
    public const string EventName = "bellman.notification.pattern.send";

    public Guid RequestId { get; set; } = Guid.NewGuid();

    public Guid? PatternId { get; init; }

    public string[] Parameters { get; init; } = [];
    public string? Content { get; init; }

    public string To { get; init; } = default!;

    public string Provider { get; init; } = default!;
}

internal sealed class SendNotificationPatternEventHandler : ICapSubscribe
{
    private readonly IEnumerable<AbstractNotificationPatternManagement> _notificationManagements;

    public SendNotificationPatternEventHandler(IEnumerable<AbstractNotificationPatternManagement> notificationManagement)
    {
        _notificationManagements = notificationManagement;
    }

    [CapSubscribe("bellman.notification.pattern.send.sms.jiring", Group = "bellman.notification.pattern.send.sms.queue")]
    public Task HandleSmsAsync(SendNotificationPatternEvent message)
    {
        var notificationManagement = _notificationManagements.First(p => p.ProviderName == message.Provider);

        return notificationManagement.SendAsync(new SendNotificationWithPatternRequest(message.RequestId)
        {
            PatternId = message.PatternId,
            Parameters = message.Parameters,
            To = [message.To]
        });
    }

    [CapSubscribe("bellman.notification.pattern.send.email.*", Group = "bellman.notification.pattern.send.email.queue")]
    public Task HandleEmailAsync(SendNotificationPatternEvent message)
    {
        var notificationManagement = _notificationManagements.First(p => p.ProviderName == message.Provider);
        return notificationManagement.SendAsync(new SendNotificationWithPatternRequest(message.RequestId)
        {
            PatternId = message.PatternId,
            Parameters = message.Parameters,
            To = [message.To]
        });
    }

    [CapSubscribe("bellman.notification.pattern.send.push.*", Group = "bellman.notification.pattern.send.push.queue")]
    public Task HandlePushNotificationAsync(SendNotificationPatternEvent message)
    {
        var notificationManagement = _notificationManagements.First(p => p.ProviderName == message.Provider);
        return notificationManagement.SendAsync(new SendNotificationWithPatternRequest(message.RequestId)
        {
            PatternId = message.PatternId,
            Parameters = message.Parameters,
            To = [message.To]
        });
    }
}