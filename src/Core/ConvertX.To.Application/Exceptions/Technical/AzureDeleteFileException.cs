using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.Application.Exceptions.Technical;

[Serializable]
[ExcludeFromCodeCoverage]
public class AzureDeleteFileException : ConvertXToTechnicalBaseException
{
    public override string Reason => "Error Deleting File From Azure";
    
    public AzureDeleteFileException() { }
    public AzureDeleteFileException(string message) : base(message) { }
    public AzureDeleteFileException(string message, Exception inner) : base(message, inner) { }
    public AzureDeleteFileException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}