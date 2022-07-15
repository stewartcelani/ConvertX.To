using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.Application.Exceptions;

[ExcludeFromCodeCoverage]
public class MsGraphUploadFileException : HttpResponseException
{
    public new static string Message => "Error uploading file to Microsoft Graph.";
    public MsGraphUploadFileException(HttpResponseMessage httpResponseMessage) : base(Message, httpResponseMessage) { }
    public MsGraphUploadFileException(HttpResponseMessage httpResponseMessage, Exception inner) : base(Message, httpResponseMessage, inner) { }

}