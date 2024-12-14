using Core.Notifications;
using FastEndpoints;
using FluentValidation;

namespace Application.Endpoints.Notifications.Send.V2;

file sealed class Endpoint : Endpoint<SendNotificationWithPatternId>
{
    private readonly INotificationService _notificationService;


    

    public Endpoint(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public override void Configure()
    {
        Post("notifications/send");
        AllowAnonymous();
        Version(2);
    }

    public override async Task HandleAsync(SendNotificationWithPatternId req, CancellationToken ct)
    {
        var res = await _notificationService.SendAsync(req, ct);
        await SendOkAsync(res, ct);
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

file sealed class RequestValidator : Validator<SendNotificationWithPatternId>
{
    public RequestValidator()
    {
        RuleFor(request => request.To)
            .NotEmpty().WithMessage("Enter Receiver (To)")
            .NotNull().WithMessage("Enter Receiver (To)");

        RuleFor(request => request.PatternId)
            .NotEmpty().WithMessage("Enter PatternId")
            .NotNull().WithMessage("Enter PatternId");

        RuleFor(request => request.Type)
            .NotEmpty().WithMessage("Enter Provider type")
            .NotNull().WithMessage("Enter Provider type");
    }
}