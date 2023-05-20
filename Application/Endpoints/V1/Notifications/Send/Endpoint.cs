using Core.Notifications;
using FastEndpoints;
using FluentValidation;

namespace Application.Endpoints.V1.Notifications.Send;

file sealed class Endpoint : Endpoint<SendNotification>
{
    private readonly INotificationService _notificationService;

    public Endpoint(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public override void Configure()
    {
        Post("notifications/send");
        Permissions("bellman_notifications_send");
        Version(1);
    }

    public override async Task HandleAsync(SendNotification req, CancellationToken ct)
    {
        await _notificationService.SendAsync(req, ct);
        await SendOkAsync(ct);
    }
}

file sealed class EndpointSummary : Summary<Endpoint>
{
    public EndpointSummary()
    {
        Summary = "Send batch notification in the system";
        Description = "Send batch notification in the system";
        Response(200, "Notification was successfully sent");
    }
}

file sealed class RequestValidator : Validator<SendNotification>
{
    public RequestValidator()
    {
        RuleFor(request => request.To)
            .NotEmpty().WithMessage("Enter Receiver (To)")
            .NotNull().WithMessage("Enter Receiver (To)");

        RuleFor(request => request.Content)
            .NotEmpty().WithMessage("Enter Content")
            .NotNull().WithMessage("Enter Content");

        RuleFor(request => request.Type)
            .NotEmpty().WithMessage("Enter Provider type")
            .NotNull().WithMessage("Enter Provider type");
    }
}