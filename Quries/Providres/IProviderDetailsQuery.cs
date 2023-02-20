namespace Quries.Providres;

public interface IProviderDetailsQuery
{
    Task<ProviderResponse> QueryAsync(Guid id, CancellationToken cancellationToken = default);
}