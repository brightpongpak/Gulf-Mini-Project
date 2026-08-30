using System.Collections.Concurrent;
using GulfUrlShortener.Api.Application;
using GulfUrlShortener.Api.Application.Interfaces;
using GulfUrlShortener.Api.Domain;

namespace GulfUrlShortener.Api.Infrastructure;

public sealed class InMemoryLinkRepository : ILinkRepository
{
    private readonly ConcurrentDictionary<string, Link> links = new(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyCollection<Link> GetAll() => links.Values.ToArray();
    public Link? Find(string code) => links.GetValueOrDefault(code);
    public bool Add(Link link) => links.TryAdd(link.Code, link);
    public bool Remove(string code) => links.TryRemove(code, out _);
}
