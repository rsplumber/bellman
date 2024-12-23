using System.Net.Http.Json;
using Core.Domains.Pattern;
using Core.Notifications;
using Core.Providers.Types;
using DotNetCore.CAP;

namespace Sms.Persiafava;

public class SendNotificationManagement : AbstractNotificationPatternManagement
{
    private const string Username = "sarmayeh";
    private const string Password = "Te@$armayeh#850";
    private const string SenderNumber = "300004373";
    private const string ApiUrl = "webservice/rest/sms_send";
    private readonly HttpClient _client;
    private readonly IPatternRepository _patternRepository;

    public SendNotificationManagement(ICapPublisher capPublisher, INotificationRepository notificationRepository, IHttpClientFactory clientFactory, IPatternRepository patternRepository) : base(capPublisher, notificationRepository, patternRepository)
    {
        _patternRepository = patternRepository;
        _client = clientFactory.CreateClient(ProviderName);
    }

    public override string ProviderName => "persiafava";

    public override string ProviderType => "sms";
    public override string ProviderTitle => "پرشیا فاوا";
    
    public override ProviderStatus ProviderStatus => ProviderStatus.Enable;
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

        var httpResponseMessage = await _client.PostAsync(ApiUrl, new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "login_username", Username },
            { "login_password", Password },
            { "receiver_number", to },
            { "note_arr", contents },
            { "note_arr[]", contents },
            { "sender_number", SenderNumber }
        }), cancellationToken);
        if (!httpResponseMessage.IsSuccessStatusCode) return null;
        var response = await httpResponseMessage.Content.ReadFromJsonAsync<PersiaFavaResponse>(cancellationToken: cancellationToken);
        if (response is null) return null;

        return new SendNotification()
        {
            MessageId = response.List.FirstOrDefault() ?? string.Empty
        };
    }

    protected override  Task<GetDeliveryNotification?> GetDeliveryStatusNotificationAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult<GetDeliveryNotification?>(new GetDeliveryNotification()
        {
            Status = 0
        });
    }

    private record PersiaFavaResponse
    {
        public List<string> List { get; set; } = default!;
    }
}