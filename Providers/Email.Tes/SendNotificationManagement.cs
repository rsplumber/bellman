using Core.Domains.Pattern;
using Core.Notifications;
using Core.Providers.Types;
using DotNetCore.CAP;
using FluentEmail.Core;
using FluentEmail.Core.Models;

namespace Emai.Tes;

internal sealed class SendNotificationManagement : AbstractNotificationPatternManagement
{
    private readonly IFluentEmail _fluentEmail;
    private readonly IPatternRepository _patternRepository;

    public SendNotificationManagement(ICapPublisher capPublisher, INotificationRepository notificationRepository, IHttpClientFactory clientFactory, IFluentEmail fluentEmail, IPatternRepository patternRepository) : base(capPublisher, notificationRepository, patternRepository)
    {
        _fluentEmail = fluentEmail;
        _patternRepository = patternRepository;
    }

    public override string ProviderName => "tes";

    public override string ProviderType => "email";
    
    public override string ProviderTitle => "سرمایه";
    public override string ProviderImage => "";

    public override ProviderStatus ProviderStatus => ProviderStatus.Disable;

    protected override int MaximumRetryCount => 0;

    protected override async Task<SendNotification?> SendNotificationAsync(Notification notification, Guid? patternId, string[]? parameters, string to, string? content, CancellationToken cancellationToken)
    {
        string contents;
        if (content is null)
        {
            var pattern = await _patternRepository.FindAsync((Guid)patternId!, cancellationToken);
            contents = string.Format(pattern?.Template ?? string.Empty, parameters.Cast<object>().ToArray());
        }
        else
        {
            contents = content;
        }

        await _fluentEmail.To(to)
            .Subject("تجارت الکترونیک سرمایه")
            .Body(contents)
            .SendAsync();
        return new SendNotification()
        {
            MessageId = ""
        };
    }

    protected override Task<GetDeliveryNotification?> GetDeliveryStatusNotificationAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult<GetDeliveryNotification?>(new GetDeliveryNotification()
        {
            Status = 0
        });
    }

    private record TesResponse
    {
        public string List { get; set; } = default!;
    }
}