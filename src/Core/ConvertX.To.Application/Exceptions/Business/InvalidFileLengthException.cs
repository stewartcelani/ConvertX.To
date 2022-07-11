using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.Application.Exceptions.Business;

[Serializable]
[ExcludeFromCodeCoverage]
public class InvalidFileLengthException : ConvertXToBusinessExceptionBase
{
    public override string Reason => "Invalid File Length";
    
    public InvalidFileLengthException() { }
    public InvalidFileLengthException(string message) : base(message) { }
    public InvalidFileLengthException(string message, Exception inner) : base(message, inner) { }
    public InvalidFileLengthException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}