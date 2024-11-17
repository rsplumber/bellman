using Core.Domains.Pattern;
using Core.Notifications;
using Data;
using FastEndpoints;
using FluentValidation;

namespace Application.Endpoints.Test;

file sealed class Endpoint : Endpoint<SendNotificationWithContent>
{
    private readonly ApplicationDbContext _dbContext;

    public Endpoint(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;

    }

    public override void Configure()
    {
        Post("test");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(SendNotificationWithContent req, CancellationToken ct)
    {
        

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

// file sealed class RequestValidator : Validator<SendNotificationWithContent>
// {
//     public RequestValidator()
//     {
//         RuleFor(request => request.To)
//             .NotEmpty().WithMessage("Enter Receiver (To)")
//             .NotNull().WithMessage("Enter Receiver (To)");
//
//         RuleFor(request => request.Content)
//             .NotEmpty().WithMessage("Enter Content")
//             .NotNull().WithMessage("Enter Content");
//
//         RuleFor(request => request.Type)
//             .NotEmpty().WithMessage("Enter Provider type")
//             .NotNull().WithMessage("Enter Provider type");
//     }
// }