using FastEndpoints;
using Queries.Providres;

namespace Application.Endpoints.V1.Providers.List;

internal sealed class Endpoint : EndpointWithoutRequest<List<ProviderResponse>>
{
    private readonly IProviderListQuery _providerListQuery;


    public Endpoint(IProviderListQuery providerListQuery)
    {
        _providerListQuery = providerListQuery;
    }

    public override void Configure()
    {
        Get("providers");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var responses = await _providerListQuery.QueryAsync(ct);
        await SendOkAsync(responses, ct);
    }
}

internal sealed class EndpointSummary : Summary<Endpoint>
{
    public EndpointSummary()
    {
        Summary = "Get provider list in the system";
        Description = "Get provider list in the system";
        Response(200, "Provider list was successfully returned");
    }
}