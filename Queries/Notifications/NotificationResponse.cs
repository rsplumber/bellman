namespace Queries.Notifications;

public sealed record NotificationResponse
{
    public Guid Id { get; init; }

    public string Content { get; init; } = default!;

    public string From { get; init; } = default!;

    public List<string> To { get; init; } = default!;

    public string Type { get; init; } = default!;

    public int Retry { get; init; }

    public string Status { get; init; } = default!;

    public DateTime CreatedDateUtc { get; init; }
}