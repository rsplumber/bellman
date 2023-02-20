namespace Core.FailedNotifications.Services;

public class FailedNotificationService : IFailedNotificationService
{
    private readonly IFailedNotificationRepository _failedNotificationRepository;

    public FailedNotificationService(IFailedNotificationRepository failedNotificationRepository)
    {
        _failedNotificationRepository = failedNotificationRepository;
    }

    public async Task CreateAsync(CreateFailedNotificationRequest req, CancellationToken cancellationToken)
    {
        var failedNotification = new FailedNotification
        {
            Id = new Guid(),
            Content = req.Content,
            From = req.From,
            To = req.To,
            ProviderType = req.ProviderType,
            ProviderId = req.ProviderId,
        };

        await _failedNotificationRepository.AddAsync(failedNotification, cancellationToken);
    }
}