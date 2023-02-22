using Core.Providers.Types;

namespace Core.Providers;

public class Provider
{
    public Guid Id { get; internal set; }

    public string Name { get; set; }

    public string Type { get; set; }

    public ProviderStatus Status { get; set; } = ProviderStatus.Disable;
    public Dictionary<string, string>? Metas { get; set; }

    public DateTime CreatedDateUtc { get; set; } = DateTime.UtcNow;
}