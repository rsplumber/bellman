namespace Core.Domains.Jirings;

public class Jiring
{
    public Jiring()
    {
    }

    public Jiring(Guid patternId, string jiringId)
    {
        PatternId = patternId;
        JiringId = jiringId;
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PatternId { get; set; }
    public string JiringId { get; set; }
}