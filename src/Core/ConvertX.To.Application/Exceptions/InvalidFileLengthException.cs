using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace ConvertX.To.Application.Exceptions;

[Serializable]
[ExcludeFromCodeCoverage]
public class InvalidFileLengthException : ConvertXToExceptionBase, IBusinessException
{
    public InvalidFileLengthException()
    {
    }

    public InvalidFileLengthException(string message) : base(message)
    {
    }

    public InvalidFileLengthException(string message, Exception inner) : base(message, inner)
    {
    }

    public InvalidFileLengthException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}