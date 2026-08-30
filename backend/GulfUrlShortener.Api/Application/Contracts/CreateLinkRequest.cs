namespace GulfUrlShortener.Api.Application.Contracts;

public sealed record CreateLinkRequest(
    string Url,
    string? Alias = null,
    string? DefaultUrl = null,
    string? IosUrl = null,
    string? AndroidUrl = null);
