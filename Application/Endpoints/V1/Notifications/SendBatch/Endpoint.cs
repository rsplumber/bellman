using Core.Events;
using DotNetCore.CAP;
using FastEndpoints;
using FluentValidation;

namespace Application.Endpoints.V1.Notifications.SendBatch;

internal sealed class Endpoint : Endpoint<Request>
{
    private readonly ICapPublisher _capPublisher;

    public Endpoint(ICapPublisher capPublisher)
    {
        _capPublisher = capPublisher;
    }

    public override void Configure()
    {
        Post("notifications/send-batch");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        for (var i = 0; i < req.To.Count; i++)
        {
            await _capPublisher.PublishAsync(ProviderSelectionEvent.EventName, new ProviderSelectionEvent
            {
                To = req.To[i],
                Content = req.Content,
                Type = req.Type
            }, cancellationToken: ct);
            await SendOkAsync(ct);
        }
    }
}

internal sealed class EndpointSummary : Summary<Endpoint>
{
    public EndpointSummary()
    {
        Summary = "Send batch notification in the system";
        Description = "Send batch notification in the system";
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

    public List<string> To { get; init; }

    public string Type { get; init; }
}