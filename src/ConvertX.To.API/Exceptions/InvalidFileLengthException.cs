using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.API.Exceptions;

[Serializable]
[ExcludeFromCodeCoverage]
public class InvalidFileLengthException : ConvertXToPublicException
{
    public override string Reason => "Invalid File Length";
    
    public InvalidFileLengthException() { }
    public InvalidFileLengthException(string message) : base(message) { }
    public InvalidFileLengthException(string message, Exception inner) : base(message, inner) { }
    public InvalidFileLengthException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}