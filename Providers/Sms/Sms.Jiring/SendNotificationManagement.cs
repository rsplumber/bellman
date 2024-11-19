using System.Buffers.Text;
using System.Net.Http.Json;
using Core.Domains.Jirings;
using Core.Domains.Pattern;
using Core.Notifications;
using DotNetCore.CAP;

namespace Sms.Jiring;

internal sealed class SendNotificationManagement : AbstractNotificationPatternManagement
{
    private const string Username = "sarmaye_41925";
    private const string Password = "YEfVjZSomtLHIPKW";
    private const string SenderNumber = "98300041925";
    private const string ApiUrl = "api/PatternMessage/send";
    private readonly HttpClient _client;
    private readonly IJiringRepository _jiringRepository;

    public SendNotificationManagement(ICapPublisher capPublisher, INotificationRepository notificationRepository, IHttpClientFactory clientFactory,
        IPatternRepository patternRepository, IJiringRepository jiringRepository) : base(capPublisher, notificationRepository, patternRepository)
    {
        _client = clientFactory.CreateClient(ProviderName);
        // var token = $"{Username}:{Password}";
        // _client.DefaultRequestHeaders.Add("Authorization", $"Basic {Base64Encode(token)}");

        var apiKey = "98U5kHpWyiJESOE92ZeUkT3RTvrlZq";
        _client.DefaultRequestHeaders.Add("x-api-key", apiKey);
        _jiringRepository = jiringRepository;
    }

    public override string ProviderName => "jiring";

    public override string ProviderType => "sms";

    protected override int MaximumRetryCount => 2;


    protected override async Task<SendNotification?> SendNotificationAsync(Guid patternId, string[] parameters, string to, CancellationToken cancellationToken)
    {
        var jiringId = await _jiringRepository.FindByPatternIdAsync(patternId, cancellationToken);
        if (jiringId is null) return null;

        var phone = to.PhoneNumberToJiringNumber();
        var httpResponseMessage = await _client.PostAsJsonAsync(ApiUrl, new
        {
            patternId = jiringId.JiringId,
            parameters = parameters,
            destinations = new[] { phone },
        }, cancellationToken);
        if (!httpResponseMessage.IsSuccessStatusCode) return null;
        var response = await httpResponseMessage.Content.ReadFromJsonAsync<JiringResponse>(cancellationToken: cancellationToken);
        if (response == null) return null;

        return new SendNotification()
        {
            Date = response.Data.Select(s => s).ToList()
        };
    }

    protected override async Task<bool> SendBatchNotificationAsync(Guid patternId, string[] parameters, string[] to, CancellationToken cancellationToken)
    {
        var jiringId = await _jiringRepository.FindByPatternIdAsync(patternId, cancellationToken);
        if (jiringId is null) return false;

        var httpResponseMessage = await _client.PostAsJsonAsync(ApiUrl, new
        {
            patternId = jiringId.JiringId,
            parameters = parameters,
            destinations = to,
        }, cancellationToken);

        return httpResponseMessage.IsSuccessStatusCode;
    }

    private record JiringResponse
    {
        public List<string> Data { get; set; } = default!;
    }
    
}

public static class DecimalExtension
{
    public static string PhoneNumberToJiringNumber(this string value)
    {
        return value.StartsWith($"0") ? string.Concat("98", value.AsSpan(1)) : value;
    }
}