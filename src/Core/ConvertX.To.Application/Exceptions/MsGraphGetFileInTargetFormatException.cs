using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.Application.Exceptions;

[ExcludeFromCodeCoverage]
public class MsGraphGetFileInTargetFormatException : HttpResponseException
{
    public new static string Message => "Error downloading drive item in requested format from Microsoft Graph.";
    public MsGraphGetFileInTargetFormatException(HttpResponseMessage httpResponseMessage) : base(Message, httpResponseMessage) { }
    public MsGraphGetFileInTargetFormatException(HttpResponseMessage httpResponseMessage, Exception inner) : base(Message, httpResponseMessage, inner) { }
}