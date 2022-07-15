using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.Application.Exceptions;

/// <summary>
/// 
/// </summary>
[Serializable]
[ExcludeFromCodeCoverage]
public abstract class ConvertXToExceptionBase : Exception
{
    protected ConvertXToExceptionBase() { }
    protected ConvertXToExceptionBase(string message) : base(message) { }
    protected ConvertXToExceptionBase(string message, Exception inner) : base(message, inner) { }
    protected ConvertXToExceptionBase(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}