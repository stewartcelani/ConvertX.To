using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.Application.Exceptions;

[ExcludeFromCodeCoverage]
public class MsGraphAuthorizationException : HttpResponseException
{
    public MsGraphAuthorizationException(HttpResponseMessage httpResponseMessage) : base(Message, httpResponseMessage)
    {
    }

    public MsGraphAuthorizationException(HttpResponseMessage httpResponseMessage, Exception inner) : base(Message,
        httpResponseMessage, inner)
    {
    }

    public new static string Message => "Error getting authorization token from Microsoft Graph.";
}