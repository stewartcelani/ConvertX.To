using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.API.Exceptions;

/// <summary>
/// Base class for technical exceptions which will only return generic 500 internal
/// server errors to the browser via the ExceptionHandlingMiddleware
/// </summary>
[Serializable]
[ExcludeFromCodeCoverage]
public abstract class ConvertXToTechnicalBaseException : ConvertXToBaseException
{
    protected ConvertXToTechnicalBaseException() { }
    protected ConvertXToTechnicalBaseException(string message) : base(message) { }
    protected ConvertXToTechnicalBaseException(string message, Exception inner) : base(message, inner) { }
    protected ConvertXToTechnicalBaseException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}