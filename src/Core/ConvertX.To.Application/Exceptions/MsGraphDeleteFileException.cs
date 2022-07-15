using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.Application.Exceptions;

[ExcludeFromCodeCoverage]
public class MsGraphDeleteFileException : HttpResponseException
{
    public new static string Message => "Error deleting drive item from Microsoft Graph.";
    public MsGraphDeleteFileException(HttpResponseMessage httpResponseMessage) : base(Message, httpResponseMessage) { }
}