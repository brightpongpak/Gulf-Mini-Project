using GulfUrlShortener.Api.Application;
using GulfUrlShortener.Api.Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GulfUrlShortener.Api.Controllers;

[ApiController]
[Route("api/links")]
public sealed class LinksController(LinkService service) : ControllerBase
{
    [HttpPost]
    public ActionResult<LinkResponse> Create(CreateLinkRequest request)
    {
        var response = service.Create(request);
        return Created($"/api/links/{response.Code}", response);
    }

    [HttpGet]
    public ActionResult<IReadOnlyCollection<LinkResponse>> GetAll() => Ok(service.GetAll());
    [HttpGet("{code}")]
    public ActionResult<LinkResponse> Get(string code) => service.Get(code) is { } link ? Ok(link) : NotFound();
    [HttpPatch("{code}/disable")]
    public IActionResult Disable(string code) => service.Disable(code) ? NoContent() : NotFound();
    [HttpDelete("{code}")]
    public IActionResult Delete(string code) => service.Delete(code) ? NoContent() : NotFound();
}
