using Core.Providers.Types;

namespace Core.Providers;

public interface IProviderCollection
{
    IReadOnlyList<Provider> Providers { get; }

    void Add(Provider provider);
}

public class ProviderCollection : IProviderCollection
{
    private readonly List<Provider> _providers;

    public ProviderCollection()
    {
        _providers = new List<Provider>();
    }

    public IReadOnlyList<Provider> Providers => _providers;

    public void Add(Provider provider)
    {
        _providers.Add(provider);
    }
}

public class Provider
{
    public string Name { get; set; }

    public string Type { get; set; }

    public ProviderStatus Status { get; set; } = ProviderStatus.Disable;
    public Dictionary<string, string>? Metas { get; set; }

}