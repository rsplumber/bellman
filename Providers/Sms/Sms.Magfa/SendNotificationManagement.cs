using System.Net.Http.Json;
using Core.Domains.Pattern;
using Core.Notifications;
using DotNetCore.CAP;

namespace Sms.Magfa;

internal sealed class SendNotificationManagement : AbstractNotificationPatternManagement
{
    private const string Username = "sarmaye_41925";
    private const string Password = "YEfVjZSomtLHIPKW";
    private const string SenderNumber = "98300041925";
    private const string ApiUrl = "http/sms/v2/send";
    private readonly HttpClient _client;
    private readonly IPatternRepository _patternRepository;

    public SendNotificationManagement(ICapPublisher capPublisher, INotificationRepository notificationRepository, IHttpClientFactory clientFactory, IPatternRepository patternRepository) : base(capPublisher, notificationRepository, patternRepository)
    {
        _patternRepository = patternRepository;
        _client = clientFactory.CreateClient(ProviderName);
        _client.DefaultRequestHeaders.Add("Username", Username);
        _client.DefaultRequestHeaders.Add("Password", Password);
    }

    public override string ProviderName => "magfa";

    public override string ProviderType => "sms";

    protected override int MaximumRetryCount => 1;

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

        var httpResponseMessage = await _client.PostAsJsonAsync(ApiUrl, new
        {
            senders = new List<string> { SenderNumber },
            messages = new List<string> { contents },
            recipients = to,
        }, cancellationToken);
        if (!httpResponseMessage.IsSuccessStatusCode) return null;
        var response = await httpResponseMessage.Content.ReadFromJsonAsync<MagfaResponse>(cancellationToken: cancellationToken);
        if (response is null) return null;

        return new SendNotification()
        {
            MessageId = response.List.FirstOrDefault() ?? string.Empty
        };
    }

    protected override Task<GetDeliveryNotification?> GetDeliveryStatusNotificationAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult<GetDeliveryNotification?>(new GetDeliveryNotification()
        {
            Status = 0
        });
    }


    private record MagfaResponse
    {
        public List<string> List { get; set; } = default!;
    }
}