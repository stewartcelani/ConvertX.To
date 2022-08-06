using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.Application.Exceptions;

[ExcludeFromCodeCoverage]
public class MsGraphDeleteFileException : HttpResponseException
{
    public MsGraphDeleteFileException(HttpResponseMessage httpResponseMessage) : base(Message, httpResponseMessage)
    {
    }

    public new static string Message => "Error deleting drive item from Microsoft Graph.";
}