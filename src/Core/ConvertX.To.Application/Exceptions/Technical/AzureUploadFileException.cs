using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.Application.Exceptions.Technical;

[Serializable]
[ExcludeFromCodeCoverage]
public class AzureUploadFileException : ConvertXToTechnicalBaseException
{
    public override string Reason => "Azure Upload Error";
    
    public AzureUploadFileException() { }
    public AzureUploadFileException(string message) : base(message) { }
    public AzureUploadFileException(string message, Exception inner) : base(message, inner) { }
    public AzureUploadFileException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}