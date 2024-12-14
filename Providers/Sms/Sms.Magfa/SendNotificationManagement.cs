using System.Net.Http.Json;
using Core.Notifications;
using DotNetCore.CAP;

namespace Sms.Magfa;

internal sealed class SendNotificationManagement : AbstractNotificationManagement
{
    private const string Username = "sarmaye_41925";
    private const string Password = "YEfVjZSomtLHIPKW";
    private const string SenderNumber = "98300041925";
    private const string ApiUrl = "http/sms/v2/send";
    private readonly HttpClient _client;

    public SendNotificationManagement(ICapPublisher capPublisher, INotificationRepository notificationRepository, IHttpClientFactory clientFactory) : base(capPublisher, notificationRepository)
    {
        _client = clientFactory.CreateClient(ProviderName);
        _client.DefaultRequestHeaders.Add("Username", Username);
        _client.DefaultRequestHeaders.Add("Password", Password);
    }

    public override string ProviderName => "magfa";

    public override string ProviderType => "sms";

    protected override int MaximumRetryCount => 0;

    protected override async Task<bool> SendNotificationAsync(string content, string to, CancellationToken cancellationToken = default)
    {
        var httpResponseMessage = await _client.PostAsJsonAsync(ApiUrl, new
        {
            senders = new List<string> { SenderNumber },
            messages = new List<string> { content },
            recipients = new[] { to },
        }, cancellationToken);
        return httpResponseMessage.IsSuccessStatusCode;
    }

    protected override async Task<bool> SendBatchNotificationAsync(string content, string[] to, CancellationToken cancellationToken = default)
    {
        var httpResponseMessage = await _client.PostAsJsonAsync(ApiUrl, new
        {
            senders = new List<string> { SenderNumber },
            messages = new List<string> { content },
            recipients = to,
        }, cancellationToken);
        return httpResponseMessage.IsSuccessStatusCode;
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