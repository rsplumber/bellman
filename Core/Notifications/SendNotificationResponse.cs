namespace Core.Notifications;

public class SendNotificationResponse
{
    
    public required SendNotificationResponseModel Data { get; init; }


    public record SendNotificationResponseModel
    {
        public required string PhoneNumber { get; init; }
        public Guid Id { get; init; }
    }
    
}



