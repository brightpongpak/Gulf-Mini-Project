using GulfUrlShortener.Api.Application;
using GulfUrlShortener.Api.Application.Contracts;
using GulfUrlShortener.Api.Application.Interfaces;
using GulfUrlShortener.Api.Application.Options;
using GulfUrlShortener.Api.Application.Exceptions;
using GulfUrlShortener.Api.Infrastructure;
using Microsoft.Extensions.Options;

namespace GulfUrlShortener.Tests;

public sealed class LinkServiceTests
{
    private static LinkService CreateService(IShortCodeGenerator? generator = null) => new(new InMemoryLinkRepository(), generator ?? new RandomShortCodeGenerator(), Options.Create(new ShortUrlOptions { BaseUrl = "http://localhost:5000" }));

    [Fact]
    public void Create_UsesCustomAliasAndBuildsShortUrl()
    {
        var link = CreateService().Create(new CreateLinkRequest("https://example.com", "demo"));
        Assert.Equal("demo", link.Code);
        Assert.True(link.IsCustomAlias);
        Assert.Equal("http://localhost:5000/r/demo", link.ShortUrl);
    }

    [Theory]
    [InlineData("example.com")]
    [InlineData("ftp://example.com/file")]
    [InlineData("not a url")]
    public void ValidateUrl_RejectsInvalidValues(string value) => Assert.Throws<InvalidUrlException>(() => LinkService.ValidateUrl(value));

    [Fact]
    public void Create_UsesGeneratedCodeWhenAliasIsMissing()
    {
        var link = CreateService(new FixedGenerator()).Create(new CreateLinkRequest("https://example.com"));
        Assert.Equal("fixed1", link.Code);
        Assert.False(link.IsCustomAlias);
    }

    [Fact]
    public void Create_RejectsDuplicateAlias()
    {
        var service = CreateService();
        service.Create(new CreateLinkRequest("https://example.com", "same"));
        Assert.Throws<DuplicateCodeException>(() => service.Create(new CreateLinkRequest("https://other.example", "same")));
    }

    [Fact]
    public void Resolve_IncrementsClicksAndChoosesAndroidDestination()
    {
        var service = CreateService();
        var created = service.Create(new CreateLinkRequest("https://default.example", AndroidUrl: "https://android.example/app"));
        var destination = service.Resolve(created.Code, "Mozilla/5.0 Android 14");
        var details = service.Get(created.Code)!;
        Assert.Equal("https://android.example/app", destination.ToString());
        Assert.Equal(1, details.Clicks);
        Assert.NotNull(details.LastAccessedAt);
    }

    [Fact]
    public void Resolve_ChoosesIosDestinationAndDefaultFallback()
    {
        var service = CreateService();
        var created = service.Create(new CreateLinkRequest("https://original.example", DefaultUrl: "https://default.example", IosUrl: "https://ios.example"));
        Assert.Equal("https://ios.example/", service.Resolve(created.Code, "Mozilla/5.0 iPhone").ToString());
        Assert.Equal("https://default.example/", service.Resolve(created.Code, "Mozilla/5.0 Windows").ToString());
    }

    [Fact]
    public async Task Resolve_CountsConcurrentClicksWithoutLosingUpdates()
    {
        var service = CreateService();
        var created = service.Create(new CreateLinkRequest("https://example.com", "parallel"));
        var requests = Enumerable.Range(0, 100).Select(_ => Task.Run(() => service.Resolve(created.Code, null)));
        await Task.WhenAll(requests);
        Assert.Equal(100, service.Get(created.Code)!.Clicks);
    }

    [Fact]
    public void DisabledLink_DoesNotResolveOrIncrementClicks()
    {
        var service = CreateService();
        var created = service.Create(new CreateLinkRequest("https://example.com", "off"));
        Assert.True(service.Disable(created.Code));
        Assert.Throws<InvalidOperationException>(() => service.Resolve(created.Code, null));
        Assert.Equal(0, service.Get(created.Code)!.Clicks);
    }

    [Fact]
    public void Delete_RemovesLink()
    {
        var service = CreateService();
        var created = service.Create(new CreateLinkRequest("https://example.com", "gone"));
        Assert.True(service.Delete(created.Code));
        Assert.Null(service.Get(created.Code));
        Assert.Throws<KeyNotFoundException>(() => service.Resolve(created.Code, null));
    }

    private sealed class FixedGenerator : IShortCodeGenerator
    {
        public string Generate() => "fixed1";
    }
}
