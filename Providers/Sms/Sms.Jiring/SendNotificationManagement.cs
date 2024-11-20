using System.Buffers.Text;
using System.Net.Http.Json;
using Core.Domains.Jirings;
using Core.Domains.Jirings.Notification;
using Core.Domains.Pattern;
using Core.Notifications;
using Core.Notifications.Types;
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
    private readonly IJiringNotificationRepository _jiringNotificationRepository;

    public SendNotificationManagement(ICapPublisher capPublisher, INotificationRepository notificationRepository, IHttpClientFactory clientFactory,
        IPatternRepository patternRepository, IJiringRepository jiringRepository, IJiringNotificationRepository jiringNotificationRepository) : base(capPublisher, notificationRepository, patternRepository)
    {
        _client = clientFactory.CreateClient(ProviderName);
        // var token = $"{Username}:{Password}";
        // _client.DefaultRequestHeaders.Add("Authorization", $"Basic {Base64Encode(token)}");

        var apiKey = "98U5kHpWyiJESOE92ZeUkT3RTvrlZq";
        _client.DefaultRequestHeaders.Add("x-api-key", apiKey);
        _jiringRepository = jiringRepository;
        _jiringNotificationRepository = jiringNotificationRepository;
    }

    public override string ProviderName => "jiring";

    public override string ProviderType => "sms";

    protected override int MaximumRetryCount => 2;


    protected override async Task<SendNotification?> SendNotificationAsync(Notification notification, Guid? patternId, string[]? parameters, string to, string? content, CancellationToken cancellationToken)
    {
        if (patternId is null)
        {
            return null;
        }

        var jiringId = await _jiringRepository.FindByPatternIdAsync((Guid)patternId, cancellationToken);
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
        if (response != null)
        {
            await _jiringNotificationRepository.AddAsync(new JiringNotification()
            {
                Notification = notification,
                MessageId = response.Data.FirstOrDefault() ?? string.Empty
            }, cancellationToken);
        }
        else
        {
            return null;
        }

        return new SendNotification()
        {
            MessageId = response.Data.FirstOrDefault() ?? string.Empty
        };
    }

    protected override async Task<GetDeliveryNotification?> GetDeliveryStatusNotificationAsync(Guid id, CancellationToken cancellationToken)
    {
        var jiring = await _jiringNotificationRepository.FindByNotificationIdAsync(id, cancellationToken);
        if (jiring is null)
        {
            return new GetDeliveryNotification()
            {
                Status = NotificationDeliveryStatus.Unknown
            };
        }

        var httpResponseMessage = await _client.PostAsJsonAsync("api/message/getdlr", new string[]
        {
            jiring.MessageId
        }, cancellationToken);
        if (!httpResponseMessage.IsSuccessStatusCode) return null;
        var response = await httpResponseMessage.Content.ReadFromJsonAsync<JiringDeliveryResponse>(cancellationToken: cancellationToken);

        if (response is null) return null;

        var status = response.Data.FirstOrDefault()?.DeliveryStatus switch
        {
            1 => NotificationDeliveryStatus.Delivered,
            2 => NotificationDeliveryStatus.UnDelivered,
            9 => NotificationDeliveryStatus.Sent,
            _ => NotificationDeliveryStatus.Unknown
        };
        return new GetDeliveryNotification()
        {
            Status = status
        };
    }

    // protected override async Task<SendNotification?> SendBatchNotificationAsync(Guid patternId, string[] parameters, string[] to, CancellationToken cancellationToken)
    // {
    //     var jiringId = await _jiringRepository.FindByPatternIdAsync(patternId, cancellationToken);
    //     if (jiringId is null) return null;
    //
    //     to = to.Select(phoneNumber => phoneNumber.PhoneNumberToJiringNumber()).ToArray();
    //     var httpResponseMessage = await _client.PostAsJsonAsync(ApiUrl, new
    //     {
    //         patternId = jiringId.JiringId,
    //         parameters = parameters,
    //         destinations = to,
    //     }, cancellationToken);
    //     if (!httpResponseMessage.IsSuccessStatusCode) return null;
    //     
    //     var response = await httpResponseMessage.Content.ReadFromJsonAsync<JiringResponse>(cancellationToken: cancellationToken);
    //     if (response == null) return null;
    //     
    //     return new SendNotification()
    //     {
    //         Date = response.Data.Select(s => s).ToList()
    //     };
    // }

    private record JiringResponse
    {
        public List<string> Data { get; set; } = default!;
    }

    private record JiringDeliveryResponse
    {
        public List<JiringDeliveryResponseData> Data { get; set; } = default!;

        public record JiringDeliveryResponseData
        {
            public int DeliveryStatus { get; set; } = default!;
            public DateTime? DeliveryDate { get; set; }
        }
    }
}

public static class DecimalExtension
{
    public static string PhoneNumberToJiringNumber(this string value)
    {
        return value.StartsWith($"0") ? string.Concat("98", value.AsSpan(1)) : value;
    }
}