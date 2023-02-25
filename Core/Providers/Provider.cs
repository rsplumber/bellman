using Core.Providers.Types;

namespace Core.Providers;

public class Provider
{
    public string Name { get; set; }

    public string Type { get; set; }

    public ProviderStatus Status { get; set; } = ProviderStatus.Disable;
    public Dictionary<string, string>? Metas { get; set; }
}