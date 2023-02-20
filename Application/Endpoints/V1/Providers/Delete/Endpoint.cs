using Core.Providers.Services;
using FastEndpoints;
using FluentValidation;

namespace Application.Endpoints.V1.Providers.Delete;

internal sealed class Endpoint : Endpoint<DeleteProviderRequest>
{
    private readonly IProviderService _providerService;


    public Endpoint(IProviderService providerService)
    {
        _providerService = providerService;
    }

    public override void Configure()
    {
        Delete("providers");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(DeleteProviderRequest req, CancellationToken ct)
    {
        await _providerService.DeleteAsync(req, ct);
        await SendOkAsync(ct);
    }
}

internal sealed class EndpointSummary : Summary<Endpoint>
{
    public EndpointSummary()
    {
        Summary = "Delete provider in the system";
        Description = "Delete provider in the system";
        Response(200, "Provider was successfully deleted");
    }
}

internal sealed class RequestValidator : Validator<DeleteProviderRequest>
{
    public RequestValidator()
    {
        RuleFor(request => request.Id)
            .NotEmpty().WithMessage("Enter ProviderId")
            .NotNull().WithMessage("Enter ProviderId");
    }
}