using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.Application.Exceptions;

/// <summary>
/// Base class for business logic exceptions which will be translated into error responses
/// with message passed via the ExceptionHandlingMiddleware
/// </summary>
[Serializable]
[ExcludeFromCodeCoverage]
public abstract class ConvertXToBusinessBaseException : ConvertXToBaseException
{
    protected ConvertXToBusinessBaseException() { }
    protected ConvertXToBusinessBaseException(string message) : base(message) { }
    protected ConvertXToBusinessBaseException(string message, Exception inner) : base(message, inner) { }
    protected ConvertXToBusinessBaseException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}