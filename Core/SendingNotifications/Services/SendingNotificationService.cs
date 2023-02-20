using Core.SendingNotifications.Exceptions;

namespace Core.SendingNotifications.Services;

public class SendingNotificationService : ISendingNotificationService
{
    private readonly ISendingNotificationRepository _sendingNotificationRepository;

    public SendingNotificationService(ISendingNotificationRepository sendingNotificationRepository)
    {
        _sendingNotificationRepository = sendingNotificationRepository;
    }

    public async Task CreateAsync(CreateSendingNotificationRequest req, CancellationToken cancellationToken)
    {
        var sendingNotification = new SendingNotification
        {
            Id = new Guid(),
            Content = req.Content,
            From = req.From,
            To = req.To,
            ProviderType = req.ProviderType,
            ProviderId = req.ProviderId,
        };

        await _sendingNotificationRepository.AddAsync(sendingNotification, cancellationToken);
    }

    public async Task DeleteAsync(DeleteSendingNotificationRequest req, CancellationToken cancellationToken = default)
    {
        var sendingNotification = await _sendingNotificationRepository.FindAsync(req.Id, cancellationToken);
        if (sendingNotification is null)
        {
            throw new SendingNotificationNotFoundException();
        }

        await _sendingNotificationRepository.DeleteAsync(sendingNotification, cancellationToken);
    }
}