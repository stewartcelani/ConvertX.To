using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.Application.Exceptions.Technical;

[Serializable]
[ExcludeFromCodeCoverage]
public class AzureAuthorizationException : ConvertXToTechnicalBaseException
{
    public override string Reason => "Error Getting Authentication Token From Azure";
    
    public AzureAuthorizationException() { }
    public AzureAuthorizationException(string message) : base(message) { }
    public AzureAuthorizationException(string message, Exception inner) : base(message, inner) { }
    public AzureAuthorizationException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}