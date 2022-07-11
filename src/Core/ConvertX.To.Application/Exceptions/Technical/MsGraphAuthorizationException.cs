using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.Application.Exceptions.Technical;

[Serializable]
[ExcludeFromCodeCoverage]
public class MsGraphAuthorizationException : ConvertXToTechnicalExceptionBase
{
    public override string Reason => "Error Getting Authentication Token From Azure";
    
    public MsGraphAuthorizationException() { }
    public MsGraphAuthorizationException(string message) : base(message) { }
    public MsGraphAuthorizationException(string message, Exception inner) : base(message, inner) { }
    public MsGraphAuthorizationException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}