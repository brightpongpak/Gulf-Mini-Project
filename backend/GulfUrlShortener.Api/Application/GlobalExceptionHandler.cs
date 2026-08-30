using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GulfUrlShortener.Api.Application.Exceptions;

namespace GulfUrlShortener.Api.Application;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            InvalidUrlException => (StatusCodes.Status400BadRequest, "Invalid URL or alias"),
            DuplicateCodeException => (StatusCodes.Status409Conflict, "Duplicate short code"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Short link not found"),
            InvalidOperationException => (StatusCodes.Status410Gone, "Short link is unavailable"),
            _ => (StatusCodes.Status500InternalServerError, "Unexpected server error")
        };

        if (status == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Unhandled exception while processing request");

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        }, cancellationToken);
        return true;
    }
}
