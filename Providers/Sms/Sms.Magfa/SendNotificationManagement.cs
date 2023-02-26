using System.Net.Http.Headers;
using System.Text;
using Core.NotificationManagements;
using Core.Notifications;
using DotNetCore.CAP;
using Newtonsoft.Json;

namespace Sms.Magfa;

public class SendNotificationManagement : AbstractNotificationManagement
{
    public override string ProviderName => "magfa";

    public override string ProviderType => "sms";

    protected override int MaximumRetryCount => 2;

    private const string Username = "sarmaye_41925";

    private const string Password = "YEfVjZSomtLHIPKW";

    private const string ApiUrl = "http/sms/v2/send";

    private const string SenderNumber = "98300041925";

    private readonly IHttpClientFactory _clientFactory;


    protected override async Task<bool> SendNotificationAsync(string content, string to, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient("magfa");
        client.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));

        client.DefaultRequestHeaders.Add("Username", Username);
        client.DefaultRequestHeaders.Add("Password", Password);

        var request = new
        {
            senders = new List<string> {SenderNumber},
            messages = new List<string> {content},
            recipients = new[] {to},
        };

        var httpRequest = new StringContent(JsonConvert.SerializeObject(request),
            Encoding.UTF8,
            "application/json");
        var httpResponseMessage = await client.PostAsync(ApiUrl, httpRequest, cancellationToken);
        return httpResponseMessage.IsSuccessStatusCode;
    }

    public SendNotificationManagement(ICapPublisher capPublisher, INotificationRepository notificationRepository, IHttpClientFactory clientFactory) : base(capPublisher, notificationRepository)
    {
        _clientFactory = clientFactory;
    }
}