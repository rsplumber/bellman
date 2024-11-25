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

    public List<Parameter>? Parameters { get; init; }

    public string? Description { get; init; }

    public DateTime CreatedDateUtc { get; internal set; } = DateTime.UtcNow;


    public record Parameter
    {
        public string? Key { get; init; }
        public string? Value { get; init; }
    }
}