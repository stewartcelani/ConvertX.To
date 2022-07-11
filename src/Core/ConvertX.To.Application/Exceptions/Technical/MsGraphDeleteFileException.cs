using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.Application.Exceptions.Technical;

[Serializable]
[ExcludeFromCodeCoverage]
public class MsGraphDeleteFileException : ConvertXToTechnicalExceptionBase
{
    public override string Reason => "Error Deleting File From Azure";
    
    public MsGraphDeleteFileException() { }
    public MsGraphDeleteFileException(string message) : base(message) { }
    public MsGraphDeleteFileException(string message, Exception inner) : base(message, inner) { }
    public MsGraphDeleteFileException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}