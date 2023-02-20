namespace Quries.Providres;

public class ProviderResponse
{
    public Guid Id { get; internal set; }

    public string Name { get; internal set; }

    public string Type { get; set; }

    public Dictionary<string, string> Metas { get; set; }

    public DateTime CreatedDateUtc { get; } = DateTime.UtcNow;
}