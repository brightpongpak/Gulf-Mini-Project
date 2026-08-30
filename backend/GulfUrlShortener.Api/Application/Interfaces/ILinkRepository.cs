using GulfUrlShortener.Api.Domain;

namespace GulfUrlShortener.Api.Application.Interfaces;

public interface ILinkRepository
{
    IReadOnlyCollection<Link> GetAll();
    Link? Find(string code);
    bool Add(Link link);
    bool Remove(string code);
}
