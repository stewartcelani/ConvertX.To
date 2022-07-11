using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.Application.Exceptions.Technical;

[Serializable]
[ExcludeFromCodeCoverage]
public class MsGraphUploadFileException : ConvertXToTechnicalExceptionBase
{
    public override string Reason => "Azure Upload Error";
    
    public MsGraphUploadFileException() { }
    public MsGraphUploadFileException(string message) : base(message) { }
    public MsGraphUploadFileException(string message, Exception inner) : base(message, inner) { }
    public MsGraphUploadFileException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}