using System.Net.Http.Json;
using Core.Notifications;
using DotNetCore.CAP;

namespace Sms.Magfa;

internal sealed class SendNotificationManagement : AbstractNotificationManagement
{
    private const string Username = "sarmaye_41925";
    private const string Password = "YEfVjZSomtLHIPKW";
    private const string SenderNumber = "98300041925";
    private readonly IHttpClientFactory _clientFactory;
    private const string ApiUrl = "http/sms/v2/send";

    public SendNotificationManagement(ICapPublisher capPublisher, INotificationRepository notificationRepository, IHttpClientFactory clientFactory) : base(capPublisher, notificationRepository)
    {
        _clientFactory = clientFactory;
    }

    public override string ProviderName => "magfa";

    public override string ProviderType => "sms";

    protected override int MaximumRetryCount => 2;

    protected override async Task<bool> SendNotificationAsync(string content, string to, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient("magfa");
        client.DefaultRequestHeaders.Add("Username", Username);
        client.DefaultRequestHeaders.Add("Password", Password);
        var httpResponseMessage = await client.PostAsJsonAsync(ApiUrl, new
        {
            senders = new List<string> { SenderNumber },
            messages = new List<string> { content },
            recipients = new[] { to },
        }, cancellationToken);
        return httpResponseMessage.IsSuccessStatusCode;
    }

    protected override async Task<bool> SendBatchNotificationAsync(string content, string[] to, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient("magfa");
        client.DefaultRequestHeaders.Add("Username", Username);
        client.DefaultRequestHeaders.Add("Password", Password);
        var httpResponseMessage = await client.PostAsJsonAsync(ApiUrl, new
        {
            senders = new List<string> { SenderNumber },
            messages = new List<string> { content },
            recipients = to,
        }, cancellationToken);
        return httpResponseMessage.IsSuccessStatusCode;
    }
}