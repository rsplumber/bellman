using Core.Notifications;
using DotNetCore.CAP;
using FluentEmail.Core;
using FluentEmail.Core.Models;

namespace Emai.Tes;

internal sealed class SendNotificationManagement : AbstractNotificationManagement
{
    private const string MailAddress = "card@tes.ir";
    private readonly IFluentEmail _fluentEmail;

    public SendNotificationManagement(ICapPublisher capPublisher, INotificationRepository notificationRepository, IHttpClientFactory clientFactory, IFluentEmail fluentEmail) : base(capPublisher, notificationRepository)
    {
        _fluentEmail = fluentEmail;
    }

    public override string ProviderName => "tes";

    public override string ProviderType => "email";

    protected override int MaximumRetryCount => 2;

    protected override async Task<bool> SendNotificationAsync(string content, string to, CancellationToken cancellationToken)
    {
        await _fluentEmail.To(to)
            .SetFrom(MailAddress)
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
            .SetFrom(MailAddress)
            .Body(content)
            .SendAsync();
        return true;
    }
}