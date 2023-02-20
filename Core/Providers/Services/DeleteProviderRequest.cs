namespace Core.Providers.Services;

public sealed record DeleteProviderRequest
{
    public Guid Id { get; set; }
}