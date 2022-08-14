using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.Application.Exceptions;

[ExcludeFromCodeCoverage]
public class HttpResponseException : ConvertXToExceptionBase
{
    protected HttpResponseException(string message, HttpResponseMessage httpResponseMessage) : base(message)
    {
        HttpResponseMessage = httpResponseMessage ?? throw new NullReferenceException(nameof(httpResponseMessage));
    }

    protected HttpResponseException(string message, HttpResponseMessage httpResponseMessage, Exception inner) : base(
        message, inner)
    {
        HttpResponseMessage = httpResponseMessage ?? throw new NullReferenceException(nameof(httpResponseMessage));
    }

    public HttpResponseMessage HttpResponseMessage { get; }
}