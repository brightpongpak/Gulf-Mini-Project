namespace GulfUrlShortener.Api.Domain;

public sealed class Link
{
    public required string Code { get; init; }
    public bool IsCustomAlias { get; init; }
    public required string OriginalUrl { get; init; }
    public string? DefaultUrl { get; init; }
    public string? IosUrl { get; init; }
    public string? AndroidUrl { get; init; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    private readonly object accessLock = new();
    public DateTimeOffset? LastAccessedAt { get; private set; }
    public long Clicks { get; private set; }
    public bool IsDisabled { get; private set; }
    public void RegisterClick()
    {
        lock (accessLock)
        {
            Clicks++;
            LastAccessedAt = DateTimeOffset.UtcNow;
        }
    }
    public void Disable() => IsDisabled = true;
}
