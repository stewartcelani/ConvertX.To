using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.API.Exceptions;

[Serializable]
[ExcludeFromCodeCoverage]
public abstract class ConvertXToInternalException : ConvertXToBaseException
{
    protected ConvertXToInternalException() { }
    protected ConvertXToInternalException(string message) : base(message) { }
    protected ConvertXToInternalException(string message, Exception inner) : base(message, inner) { }
    protected ConvertXToInternalException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}