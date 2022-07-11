using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.Application.Exceptions;

/// <summary>
/// Base class for technical exceptions which will only return generic 500 internal
/// server errors to the browser via the ExceptionHandlingMiddleware
/// </summary>
[Serializable]
[ExcludeFromCodeCoverage]
public abstract class ConvertXToTechnicalExceptionBase : ConvertXToExceptionBase
{
    protected ConvertXToTechnicalExceptionBase() { }
    protected ConvertXToTechnicalExceptionBase(string message) : base(message) { }
    protected ConvertXToTechnicalExceptionBase(string message, Exception inner) : base(message, inner) { }
    protected ConvertXToTechnicalExceptionBase(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}