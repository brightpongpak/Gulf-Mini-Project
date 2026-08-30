namespace GulfUrlShortener.Api.Application.Exceptions;

public sealed class InvalidUrlException(string message) : Exception(message);
public sealed class DuplicateCodeException(string message) : Exception(message);
