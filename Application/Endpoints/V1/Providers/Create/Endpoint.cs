using Core.Providers.Services;
using FastEndpoints;
using FluentValidation;

namespace Application.Endpoints.V1.Providers.Create;

internal sealed class Endpoint : Endpoint<CreateProviderRequest>
{
    private readonly IProviderService _providerService;


    public Endpoint(IProviderService providerService)
    {
        _providerService = providerService;
    }

    public override void Configure()
    {
        Post("providers");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(CreateProviderRequest req, CancellationToken ct)
    {
        await _providerService.CreateAsync(req, ct);
        await SendOkAsync(ct);
    }
}

internal sealed class EndpointSummary : Summary<Endpoint>
{
    public EndpointSummary()
    {
        Summary = "Create provider in the system";
        Description = "Create provider in the system";
        Response(200, "Provider was successfully created");
    }
}

internal sealed class RequestValidator : Validator<CreateProviderRequest>
{
    public RequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("Enter Name")
            .NotNull().WithMessage("Enter Name");

        RuleFor(request => request.Type)
            .NotEmpty().WithMessage("Enter Provider type")
            .NotNull().WithMessage("Enter Provider type");
    }
}