using FastEndpoints;
using FluentValidation;
using Queries.Providers;

namespace Application.Endpoints.Providers.Detail;

file sealed class Endpoint : Endpoint<Request, ProviderResponse>
{
    private readonly IProviderDetailsQuery _providerDetailsQuery;

    public Endpoint(IProviderDetailsQuery providerDetailsQuery)
    {
        _providerDetailsQuery = providerDetailsQuery;
    }

    public override void Configure()
    {
        Get("providers/{name}");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var response = await _providerDetailsQuery.QueryAsync(req.Name, ct);
        await SendOkAsync(response, ct);
    }
}

file sealed class EndpointSummary : Summary<Endpoint>
{
    public EndpointSummary()
    {
        Summary = "Get provider detail in the system";
        Description = "Get provider detail in the system";
        Response(200, "Provider detail was successfully returned");
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