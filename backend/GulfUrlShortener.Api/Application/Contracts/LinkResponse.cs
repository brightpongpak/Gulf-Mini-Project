using GulfUrlShortener.Api.Domain;

namespace GulfUrlShortener.Api.Application.Contracts;

public sealed record LinkResponse(
    string Code,
    bool IsCustomAlias,
    string ShortUrl,
    string OriginalUrl,
    string? DefaultUrl,
    string? IosUrl,
    string? AndroidUrl,
    long Clicks,
    DateTimeOffset CreatedAt,
    DateTimeOffset? LastAccessedAt,
    bool IsDisabled)
{
    public static LinkResponse From(Link link, string baseUrl) => new(
        link.Code,
        link.IsCustomAlias,
        $"{baseUrl.TrimEnd('/')}/r/{link.Code}",
        link.OriginalUrl,
        link.DefaultUrl,
        link.IosUrl,
        link.AndroidUrl,
        link.Clicks,
        link.CreatedAt,
        link.LastAccessedAt,
        link.IsDisabled);
}
