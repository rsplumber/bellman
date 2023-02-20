namespace Quries.Providres;

public interface IProviderListQuery
{
    Task<List<ProviderResponse>> QueryAsync(CancellationToken cancellationToken = default);
}