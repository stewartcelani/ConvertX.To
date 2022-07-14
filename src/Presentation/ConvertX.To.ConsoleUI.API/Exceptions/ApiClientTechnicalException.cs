using Refit;

namespace ConvertX.To.ConsoleUI.API.Exceptions;

public class ApiClientTechnicalException : ApiClientExceptionBase
{
    public override string Reason => "Api Client Technical Exception";
    
    public ApiClientTechnicalException() { }
    public ApiClientTechnicalException(string message) : base(message) { }
    public ApiClientTechnicalException(string message, Exception inner) : base(message, inner) { }
    public ApiClientTechnicalException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    public ApiClientTechnicalException(ApiException ex) : base($"{ex.Message} @{ex.HttpMethod.Method}('{ex.Uri.AbsoluteUri}') Content: '{ex.Content}'", ex)
    {
    }
}