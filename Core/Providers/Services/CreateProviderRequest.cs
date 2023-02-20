namespace Core.Providers.Services;

public sealed record CreateProviderRequest
{
    public string Name { get; set; }

    public string Type { get; set; }

    public Dictionary<string, string> Metas { get; set; }
}