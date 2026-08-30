using GulfUrlShortener.Api.Application;
using Microsoft.AspNetCore.Mvc;

namespace GulfUrlShortener.Api.Controllers;

[ApiController]
public sealed class RedirectController(LinkService service) : ControllerBase
{
    [HttpGet("r/{code}")]
    public IActionResult Resolve(string code) => Redirect(service.Resolve(code, Request.Headers.UserAgent.ToString()).ToString());
}
