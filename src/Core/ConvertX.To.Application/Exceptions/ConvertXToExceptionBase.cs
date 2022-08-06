using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace ConvertX.To.Application.Exceptions;

/// <summary>
/// </summary>
[Serializable]
[ExcludeFromCodeCoverage]
public abstract class ConvertXToExceptionBase : Exception
{
    protected ConvertXToExceptionBase()
    {
    }

    protected ConvertXToExceptionBase(string message) : base(message)
    {
    }

    protected ConvertXToExceptionBase(string message, Exception inner) : base(message, inner)
    {
    }

    protected ConvertXToExceptionBase(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}