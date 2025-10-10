using Core.Providers;
using FastEndpoints;
using FluentValidation;
using Queries.Providers;

namespace Application.Endpoints.V1.Providers.Toggle;

file sealed class Endpoint : Endpoint<Request, ProviderResponse>
{
    private readonly IProviderService _providerService;

    public Endpoint(IProviderService providerService)
    {
        _providerService = providerService;
    }


    public override void Configure()
    {
        Patch("providers/{name}/toggle");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        await _providerService.ToggleAsync(req.Name, ct);
        await Send.OkAsync(cancellation: ct);
    }
}

file sealed class EndpointSummary : Summary<Endpoint>
{
    public EndpointSummary()
    {
        Summary = "Toggle provider";
        Description = "Toggle on/off a provider by name";
        Response(200, "Provider successfully toggled");
    }
}

file sealed record Request
{
    public string Name { get; init; } = default!;
}

file sealed class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("Enter Name")
            .NotNull().WithMessage("Enter Name");
    }
}