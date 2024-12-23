using System.Net.Http.Headers;
using Core.Notifications;
using Core.Providers.Types;
using DotNetCore.CAP;

namespace Sms.Ssmss;

public class SendNotificationManagement : AbstractNotificationManagement
{
    private const string Username = "sarmayehbulk";
    private const string Password = "Sabas0ft123@";
    private const string SenderNumber = "10007666000000";
    private readonly HttpClient _client;
    private const string ApiUrl = "rest/sms_send?";

    public SendNotificationManagement(ICapPublisher capPublisher, INotificationRepository notificationRepository, IHttpClientFactory clientFactory) : base(capPublisher, notificationRepository)
    {
        _client = clientFactory.CreateClient(ProviderName);
        _client.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
    }

    public override string ProviderName => "ssmss";

    public override string ProviderType => "sms";
    

    protected override int MaximumRetryCount => 0;

    protected override async Task<bool> SendNotificationAsync(string content, string to, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "login_username", Username },
            { "login_password", Password },
            { "receiver_number", to },
            { "note_arr", content },
            { "note_arr[]", content },
            { "sender_number", SenderNumber }
        };
        var encodedContent = new FormUrlEncodedContent(parameters);

        var httpResponseMessage = await _client.PostAsync(ApiUrl, encodedContent, cancellationToken);
        return httpResponseMessage.IsSuccessStatusCode;
    }

    protected override Task<bool> SendBatchNotificationAsync(string content, string[] to, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
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