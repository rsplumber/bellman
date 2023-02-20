namespace Core.Providers.Services;

public sealed record UpdateProviderRequest
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Type { get; set; }

    public Dictionary<string, string> Metas { get; set; }
}