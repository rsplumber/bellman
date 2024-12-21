using Core.Providers;
using FastEndpoints;
using FluentValidation;
using Queries.Providers;

namespace Application.Endpoints.Providers.Activation;

file sealed class Endpoint : Endpoint<Request, ProviderResponse>
{
    private readonly IProviderService _providerService;

    public Endpoint(IProviderService providerService)
    {
        _providerService = providerService;
    }


    public override void Configure()
    {
        Patch("providers/activation");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        await _providerService.ActivationAsync(req.Name, req.Type, ct);
        await SendOkAsync(ct);
    }
}

file sealed class EndpointSummary : Summary<Endpoint>
{
    public EndpointSummary()
    {
        Summary = "Activation provider";
        Description = "Activation a provider by name and type";
        Response(200, "Provider successfully toggled");
    }
}

file sealed record Request
{
    public string Name { get; init; } = default!;
    public string Type { get; init; } = default!;
}

file sealed class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("Enter Name")
            .NotNull().WithMessage("Enter Name");

        RuleFor(request => request.Type)
            .NotEmpty().WithMessage("Enter Type")
            .NotNull().WithMessage("Enter Type");
    }
}