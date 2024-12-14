using Core.Notifications;
using FastEndpoints;
using FluentValidation;

namespace Application.Endpoints.Notifications.Delivery.Check;

file sealed class Endpoint : Endpoint<DeliveryStatusRequest>
{
    private readonly INotificationService _notificationService;


    public Endpoint(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public override void Configure()
    {
        Post("notifications/{id}/delivery/status");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(DeliveryStatusRequest req, CancellationToken ct)
    {
        var res = await _notificationService.DeliveryStatusAsync(req, ct);
        await SendOkAsync(res, ct);
    }
}

file sealed class EndpointSummary : Summary<Endpoint>
{
    public EndpointSummary()
    {
        Summary = "get delivery notification in the system";
        Description = "get delivery notification in the system";
        Response(200, "Notification was successfully sent");
    }
}

file sealed class RequestValidator : Validator<DeliveryStatusRequest>
{
    public RequestValidator()
    {
        RuleFor(request => request.PhoneNumber)
            .NotEmpty().WithMessage("Enter PhoneNumber")
            .NotNull().WithMessage("Enter PhoneNumber");

        RuleFor(request => request.Id)
            .NotEmpty().WithMessage("Enter Id")
            .NotNull().WithMessage("Enter Id");
    }
}