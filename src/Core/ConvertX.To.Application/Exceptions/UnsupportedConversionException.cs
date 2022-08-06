using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace ConvertX.To.Application.Exceptions;

[Serializable]
[ExcludeFromCodeCoverage]
public class UnsupportedConversionException : ConvertXToExceptionBase, IBusinessException
{
    public UnsupportedConversionException()
    {
    }

    public UnsupportedConversionException(string message) : base(message)
    {
    }

    public UnsupportedConversionException(string message, Exception inner) : base(message, inner)
    {
    }

    public UnsupportedConversionException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}