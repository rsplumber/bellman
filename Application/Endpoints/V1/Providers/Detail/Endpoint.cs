using FastEndpoints;
using Queries.Providers;

namespace Application.Endpoints.V1.Providers.Detail;

internal sealed class Endpoint : Endpoint<Request, ProviderResponse>
{
    private readonly IProviderDetailsQuery _providerDetailsQuery;

    public Endpoint(IProviderDetailsQuery providerDetailsQuery)
    {
        _providerDetailsQuery = providerDetailsQuery;
    }

    public override void Configure()
    {
        Get("providers/{id}");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var response = await _providerDetailsQuery.QueryAsync(req.Name, ct);
        await SendOkAsync(response, ct);
    }
}

internal sealed class EndpointSummary : Summary<Endpoint>
{
    public EndpointSummary()
    {
        Summary = "Get provider detail in the system";
        Description = "Get provider detail in the system";
        Response(200, "Provider detail was successfully returned");
    }
}

internal sealed record Request
{
    public string Name { get; set; }
}