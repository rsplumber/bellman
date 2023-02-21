using Core.Providers.Exceptions;

namespace Core.Providers.Services;

public class ProviderService : IProviderService
{
    private readonly IProviderRepository _providerRepository;

    public ProviderService(IProviderRepository accountRepository)
    {
        _providerRepository = accountRepository;
    }


    public async Task CreateAsync(CreateProviderRequest req, CancellationToken cancellationToken = default)
    {
        var provider = new Provider
        {
            Id = new Guid(),
            Name = req.Name,
            Type = req.Type,
            Metas = req.Metas
        };

        await _providerRepository.AddAsync(provider, cancellationToken);
    }

    public async Task UpdateAsync(UpdateProviderRequest req, CancellationToken cancellationToken = default)
    {
        var provider = await _providerRepository.FindAsync(req.Id, cancellationToken);
        if (provider is null)
        {
            throw new ProviderNotFoundException();
        }

        provider.Name = req.Name;
        provider.Type = req.Type;
        provider.Metas = req.Metas;

        await _providerRepository.UpdateAsync(provider, cancellationToken);
    }

    public async Task DeleteAsync(DeleteProviderRequest req, CancellationToken cancellationToken = default)
    {
        var provider = await _providerRepository.FindAsync(req.Id, cancellationToken);
        if (provider is null)
        {
            throw new ProviderNotFoundException();
        }

        await _providerRepository.DeleteAsync(provider, cancellationToken);
    }
}