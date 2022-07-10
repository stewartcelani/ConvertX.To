using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.API.Exceptions.Business;

[Serializable]
[ExcludeFromCodeCoverage]
public class ConversionNotFoundException : ConvertXToBusinessBaseException
{
    public override string Reason => "Conversion Not Found";
    
    public ConversionNotFoundException() { }
    public ConversionNotFoundException(string message) : base(message) { }
    public ConversionNotFoundException(string message, Exception inner) : base(message, inner) { }
    public ConversionNotFoundException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}