using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.Application.Exceptions.Technical;

[Serializable]
[ExcludeFromCodeCoverage]
public class MsGraphGetFileInTargetFormatException : ConvertXToTechnicalExceptionBase
{
    public override string Reason => "Azure File Conversion Download Error";
    
    public MsGraphGetFileInTargetFormatException() { }
    public MsGraphGetFileInTargetFormatException(string message) : base(message) { }
    public MsGraphGetFileInTargetFormatException(string message, Exception inner) : base(message, inner) { }
    public MsGraphGetFileInTargetFormatException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}