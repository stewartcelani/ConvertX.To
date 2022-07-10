using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.API.Exceptions;

/// <summary>
/// 
/// </summary>
[Serializable]
[ExcludeFromCodeCoverage]
public abstract class ConvertXToBaseException : Exception
{
    public abstract string Reason { get; }
    protected ConvertXToBaseException() { }
    protected ConvertXToBaseException(string message) : base(message) { }
    protected ConvertXToBaseException(string message, Exception inner) : base(message, inner) { }
    protected ConvertXToBaseException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}