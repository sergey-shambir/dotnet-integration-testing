using System.Net;

namespace WebService.Specs.Drivers;

public class ApiClientException(HttpStatusCode code, string? message, Exception? innerException = null)
    : Exception($"HTTP status code {code}: {message}", innerException)
{
    public HttpStatusCode HttpStatusCode { get; } = code;
}