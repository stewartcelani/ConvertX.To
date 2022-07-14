using Refit;

namespace ConvertX.To.ConsoleUI.API.Exceptions;

public class ApiClientBusinessException : ApiClientExceptionBase
{
    public override string Reason => "Api Client Business Exception";
    
    public ApiClientBusinessException() { }
    public ApiClientBusinessException(string message) : base(message) { }
    public ApiClientBusinessException(string message, Exception inner) : base(message, inner) { }
    public ApiClientBusinessException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    public ApiClientBusinessException(ApiException ex) : base($"{ex.Message} @{ex.HttpMethod.Method}('{ex.Uri.AbsoluteUri}') Content: '{ex.Content}'", ex)
    {
    }
}