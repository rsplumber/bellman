using Core.Providers.Types;

namespace Core.Providers;

public class Provider
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; init; } = default!;

    public string Type { get; init; } = default!;
    
    public string Title { get; init; } = default!;
    
    public string Image { get; init; } = default!;

    public ProviderStatus Status { get; set; } = ProviderStatus.Disable;
}