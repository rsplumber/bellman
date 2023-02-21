namespace Core.Providers.Services;

public interface IProviderService
{
    Task CreateAsync(CreateProviderRequest req, CancellationToken cancellationToken = default);

    Task UpdateAsync(UpdateProviderRequest req, CancellationToken cancellationToken = default);

    Task DeleteAsync(DeleteProviderRequest req, CancellationToken cancellationToken = default);
   
}