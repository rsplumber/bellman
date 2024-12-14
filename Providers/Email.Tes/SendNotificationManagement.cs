using Core.Notifications;
using DotNetCore.CAP;
using FluentEmail.Core;
using FluentEmail.Core.Models;

namespace Emai.Tes;

internal sealed class SendNotificationManagement : AbstractNotificationManagement
{
    private readonly IFluentEmail _fluentEmail;

    public SendNotificationManagement(ICapPublisher capPublisher, INotificationRepository notificationRepository, IHttpClientFactory clientFactory, IFluentEmail fluentEmail) : base(capPublisher, notificationRepository)
    {
        _fluentEmail = fluentEmail;
    }

    public override string ProviderName => "tes";

    public override string ProviderType => "email";

    protected override int MaximumRetryCount => 0;

    protected override async Task<bool> SendNotificationAsync(string content, string to, CancellationToken cancellationToken)
    {
        await _fluentEmail.To(to)
            .Subject("تجارت الکترونیک سرمایه")
            .Body(content)
            .SendAsync();
        return true;
    }

    protected override async Task<bool> SendBatchNotificationAsync(string content, string[] to, CancellationToken cancellationToken)
    {
        await _fluentEmail.To(to.Select(s => new Address
            {
                Name = "",
                EmailAddress = s
            }))
            .Subject("تجارت الکترونیک سرمایه")
            .Body(content)
            .SendAsync();
        return true;
    }

    protected override Task<bool> SendNotificationAsync(Guid patternId, string[] parameters, string to, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override Task<bool> SendBatchNotificationAsync(Guid patternId, string[] parameters, string[] to, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}