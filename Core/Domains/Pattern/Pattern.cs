namespace Core.Domains.Pattern;

public class Pattern
{
    public Pattern()
    {
    }

    public Pattern(string template)
    {
        Template = template;
    }

    public Guid Id { get; set; } = Guid.NewGuid();

    public string Template { get; init; } = default!;

    public DateTime CreatedDateUtc { get; internal set; } = DateTime.UtcNow;
}