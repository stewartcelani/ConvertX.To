using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.API.Exceptions;

[Serializable]
[ExcludeFromCodeCoverage]
public abstract class ConvertXToPublicException : ConvertXToBaseException
{
    protected ConvertXToPublicException() { }
    protected ConvertXToPublicException(string message) : base(message) { }
    protected ConvertXToPublicException(string message, Exception inner) : base(message, inner) { }
    protected ConvertXToPublicException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}