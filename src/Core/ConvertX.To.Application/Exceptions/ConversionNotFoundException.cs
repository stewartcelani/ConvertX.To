using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace ConvertX.To.Application.Exceptions;

[Serializable]
[ExcludeFromCodeCoverage]
public class ConversionNotFoundException : ConvertXToExceptionBase, IBusinessException
{
    public ConversionNotFoundException()
    {
    }

    public ConversionNotFoundException(string message) : base(message)
    {
    }

    public ConversionNotFoundException(string message, Exception inner) : base(message, inner)
    {
    }

    public ConversionNotFoundException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}