namespace Queries.Providers;

public class ProviderResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Type { get; set; }

    public string Status { get; set; }

    public Dictionary<string, string>? Metas { get; set; }

}