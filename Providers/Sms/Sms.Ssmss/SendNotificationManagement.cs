using System.Net.Http.Headers;
using Core.NotificationManagements;
using Core.Notifications;
using DotNetCore.CAP;

namespace Sms.Ssmss;

public class SendNotificationManagement : AbstractNotificationManagement
{
    public override string ProviderName => "ssmss";

    public override string ProviderType => "sms";

    protected override int MaximumRetryCount => 2;

    private const string Username = "sarmayehbulk";

    private const string Password = "Sabas0ft123@";

    private const string ApiUrl = "rest/sms_send?";

    private const string SenderNumber = "10007666000000";

    private readonly IHttpClientFactory _clientFactory;


    protected override async Task<bool> SendNotificationAsync(string content, string to, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient("ssmss");
        client.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

        var parameters = new Dictionary<string, string>
        {
            {"login_username", Username},
            {"login_password", Password},
            {"receiver_number", to},
            {"note_arr", content},
            {"note_arr[]", content},
            {"sender_number", SenderNumber}
        };
        var encodedContent = new FormUrlEncodedContent(parameters);

        var httpResponseMessage = await client.PostAsync(ApiUrl, encodedContent, cancellationToken);
        return httpResponseMessage.IsSuccessStatusCode;
    }

    public SendNotificationManagement(ICapPublisher capPublisher, INotificationRepository notificationRepository, IHttpClientFactory clientFactory) : base(capPublisher, notificationRepository)
    {
        _clientFactory = clientFactory;
    }
}