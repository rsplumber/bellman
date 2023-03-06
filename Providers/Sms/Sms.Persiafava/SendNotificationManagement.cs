using Core.Notifications;
using DotNetCore.CAP;

namespace Sms.Persiafava;

public class SendNotificationManagement : AbstractNotificationManagement
{
    private const string Username = "sarmayeh";
    private const string Password = "Te@$armayeh#850";
    private const string SenderNumber = "300004373";
    private readonly IHttpClientFactory _clientFactory;
    private const string ApiUrl = "webservice/rest/sms_send";

    public SendNotificationManagement(ICapPublisher capPublisher, INotificationRepository notificationRepository, IHttpClientFactory clientFactory) : base(capPublisher, notificationRepository)
    {
        _clientFactory = clientFactory;
    }

    public override string ProviderName => "persiafava";

    public override string ProviderType => "sms";

    protected override int MaximumRetryCount => 2;

    protected override async Task<bool> SendNotificationAsync(string content, string to, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient("persiafava");
        var httpResponseMessage = await client.PostAsync(ApiUrl, new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "login_username", Username },
            { "login_password", Password },
            { "receiver_number", to },
            { "note_arr", content },
            { "note_arr[]", content },
            { "sender_number", SenderNumber }
        }), cancellationToken);
        return httpResponseMessage.IsSuccessStatusCode;
    }

    protected override Task<bool> SendBatchNotificationAsync(string content, string[] to, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}