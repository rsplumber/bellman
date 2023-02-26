using Core.Events;
using DotNetCore.CAP;
using FastEndpoints;
using FluentValidation;

namespace Application.Endpoints.V1.Notifications.Send;

internal sealed class Endpoint : Endpoint<Request>
{
    private readonly ICapPublisher _capPublisher;

    public Endpoint(ICapPublisher capPublisher)
    {
        _capPublisher = capPublisher;
    }

    public override void Configure()
    {
        Post("notifications/send");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        await _capPublisher.PublishAsync(ProviderSelectionEvent.EventName, new ProviderSelectionEvent
        {
            To = new[] {req.To},
            Content = req.Content,
            Type = req.Type
        }, cancellationToken: ct);
        await SendOkAsync(ct);
    }
}

internal sealed class EndpointSummary : Summary<Endpoint>
{
    public EndpointSummary()
    {
        Summary = "Send notification in the system";
        Description = "Send notification in the system";
        Response(200, "Notification was successfully sent");
    }
}

internal sealed class RequestValidator : Validator<Request>
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

internal sealed record Request
{
    public string Content { get; init; }

    public string To { get; init; }

    public string Type { get; init; }
}