using System.Buffers.Text;
using System.Net.Http.Json;
using Core.Notifications;
using DotNetCore.CAP;

namespace Sms.Jiring;

internal sealed class SendNotificationManagement : AbstractNotificationManagement
{
    private const string Username = "sarmaye_41925";
    private const string Password = "YEfVjZSomtLHIPKW";
    private const string SenderNumber = "98300041925";
    private const string ApiUrl = "api/PatternMessage/send";
    private readonly HttpClient _client;

    public SendNotificationManagement(ICapPublisher capPublisher, INotificationRepository notificationRepository, IHttpClientFactory clientFactory) : base(capPublisher, notificationRepository)
    {
        _client = clientFactory.CreateClient(ProviderName);
        // var token = $"{Username}:{Password}";
        // _client.DefaultRequestHeaders.Add("Authorization", $"Basic {Base64Encode(token)}");

        var apiKey = "98AeiArvGH8kwtaUnXfXD3/s3JYfWEJPfI925GuI8StzWKCTVBkfQxEhuZif72Dc";
        _client.DefaultRequestHeaders.Add("x-api-key", apiKey);

    }

    public override string ProviderName => "jiring";

    public override string ProviderType => "sms";

    protected override int MaximumRetryCount => 2;

    protected override async Task<bool> SendNotificationAsync(string content, string to, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> SendBatchNotificationAsync(string content, string[] to, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> SendNotificationAsync(Guid patternId, string[] parameters, string to, CancellationToken cancellationToken)
    {
        var httpResponseMessage = await _client.PostAsJsonAsync(ApiUrl, new
        {
            PatternId = patternId,
            Parameters = new [] { parameters },
            Destinations = new[] { to },
        }, cancellationToken);
        
        return httpResponseMessage.IsSuccessStatusCode;
    }

    protected override async Task<bool> SendBatchNotificationAsync(Guid patternId, string[] parameters, string[] to, CancellationToken cancellationToken)
    {
        var httpResponseMessage = await _client.PostAsJsonAsync(ApiUrl, new
        {
            PatternId = patternId,
            Parameters = new [] { parameters },
            Destinations = new[] { to },
        }, cancellationToken);
        
        return httpResponseMessage.IsSuccessStatusCode;
    }

    private static string Base64Encode(string plainText)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }
}