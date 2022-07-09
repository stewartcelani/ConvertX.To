using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.API.Exceptions;

[Serializable]
[ExcludeFromCodeCoverage]
public class AzureDownloadConvertedFileException : ConvertXToInternalException
{
    public override string Reason => "Azure File Conversion Download Error";
    
    public AzureDownloadConvertedFileException() { }
    public AzureDownloadConvertedFileException(string message) : base(message) { }
    public AzureDownloadConvertedFileException(string message, Exception inner) : base(message, inner) { }
    public AzureDownloadConvertedFileException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}