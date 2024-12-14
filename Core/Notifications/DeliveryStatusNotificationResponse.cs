using Core.Notifications.Types;

namespace Core.Notifications;

public class DeliveryStatusNotificationResponse
{
    public required DeliveryStatusNotificationResponseModel Date { get; init; }


    public record DeliveryStatusNotificationResponseModel
    {
        public NotificationDeliveryStatus? Status { get; init; }
    }
}