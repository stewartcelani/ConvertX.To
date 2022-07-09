using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.API.Exceptions;

[Serializable]
[ExcludeFromCodeCoverage]
public class AzureAuthorizationException : ConvertXToInternalException
{
    public override string Reason => "Azure Authorization Error";
    
    public AzureAuthorizationException() { }
    public AzureAuthorizationException(string message) : base(message) { }
    public AzureAuthorizationException(string message, Exception inner) : base(message, inner) { }
    public AzureAuthorizationException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}