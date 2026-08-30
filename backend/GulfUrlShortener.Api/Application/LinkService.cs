using System.Text.RegularExpressions;
using GulfUrlShortener.Api.Domain;
using GulfUrlShortener.Api.Application.Contracts;
using GulfUrlShortener.Api.Application.Exceptions;
using GulfUrlShortener.Api.Application.Interfaces;
using GulfUrlShortener.Api.Application.Options;
using Microsoft.Extensions.Options;

namespace GulfUrlShortener.Api.Application;

public sealed class LinkService(ILinkRepository repository, IShortCodeGenerator codeGenerator, IOptions<ShortUrlOptions> options)
{
    private static readonly Regex AliasPattern = new("^[A-Za-z0-9_-]{3,32}$", RegexOptions.Compiled);
    private readonly string baseUrl = options.Value.BaseUrl;

    public LinkResponse Create(CreateLinkRequest request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Url))
            throw new InvalidUrlException("A destination URL is required.");

        var urls = new[] { request.Url, request.DefaultUrl, request.IosUrl, request.AndroidUrl }.Where(value => !string.IsNullOrWhiteSpace(value));
        foreach (var url in urls)
        {
            if (url!.Length > 2048) throw new InvalidUrlException("URLs must be 2048 characters or fewer.");
            ValidateUrl(url);
        }
        var hasCustomAlias = !string.IsNullOrWhiteSpace(request.Alias);
        var code = hasCustomAlias ? ValidateAlias(request.Alias) : GenerateUniqueCode();
        var link = new Link { Code = code, IsCustomAlias = hasCustomAlias, OriginalUrl = request.Url, DefaultUrl = request.DefaultUrl, IosUrl = request.IosUrl, AndroidUrl = request.AndroidUrl };
        if (!repository.Add(link)) throw new DuplicateCodeException($"The alias '{code}' is already in use.");
        return LinkResponse.From(link, baseUrl);
    }

    public IReadOnlyCollection<LinkResponse> GetAll() => repository.GetAll().OrderByDescending(link => link.CreatedAt).Select(link => LinkResponse.From(link, baseUrl)).ToArray();
    public LinkResponse? Get(string code) => repository.Find(code) is { } link ? LinkResponse.From(link, baseUrl) : null;
    public bool Disable(string code) { var link = repository.Find(code); if (link is null) return false; link.Disable(); return true; }
    public bool Delete(string code) => repository.Remove(code);

    public Uri Resolve(string code, string? userAgent)
    {
        var link = repository.Find(code);
        if (link is null) throw new KeyNotFoundException();
        if (link.IsDisabled) throw new InvalidOperationException("This link is disabled.");
        link.RegisterClick();
        var destination = IsIos(userAgent) ? link.IosUrl : IsAndroid(userAgent) ? link.AndroidUrl : link.DefaultUrl;
        return new Uri(destination ?? link.OriginalUrl);
    }

    public static void ValidateUrl(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var parsed) || parsed.Scheme is not ("http" or "https") || string.IsNullOrWhiteSpace(parsed.Host))
            throw new InvalidUrlException($"'{url}' is not a valid HTTP or HTTPS URL.");
    }

    private static string ValidateAlias(string? alias)
    {
        if (alias is null || !AliasPattern.IsMatch(alias)) throw new InvalidUrlException("Alias must be 3-32 characters using letters, numbers, '-' or '_'.");
        return alias;
    }

    private string GenerateUniqueCode()
    {
        for (var attempt = 0; attempt < 20; attempt++) { var candidate = codeGenerator.Generate(); if (repository.Find(candidate) is null) return candidate; }
        throw new InvalidOperationException("Could not generate a unique short code.");
    }

    private static bool IsIos(string? agent) => agent?.Contains("iPhone", StringComparison.OrdinalIgnoreCase) == true || agent?.Contains("iPad", StringComparison.OrdinalIgnoreCase) == true;
    private static bool IsAndroid(string? agent) => agent?.Contains("Android", StringComparison.OrdinalIgnoreCase) == true;
}

public sealed class RandomShortCodeGenerator : IShortCodeGenerator
{
    private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789";
    public string Generate() { Span<char> result = stackalloc char[6]; for (var i = 0; i < result.Length; i++) result[i] = Alphabet[Random.Shared.Next(Alphabet.Length)]; return new string(result); }
}
