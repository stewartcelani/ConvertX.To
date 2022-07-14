using ConvertX.To.Application.Exceptions;
using Refit;

namespace ConvertX.To.ConsoleUI.API.Exceptions;

public abstract class ApiClientExceptionBase : ConvertXToExceptionBase
{
    public override string Reason => "Api Client Exception";
    
    protected ApiClientExceptionBase() { }
    protected ApiClientExceptionBase(string message) : base(message) { }
    protected ApiClientExceptionBase(string message, Exception inner) : base(message, inner) { }
    protected ApiClientExceptionBase(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    protected ApiClientExceptionBase(ApiException ex) : base($"{ex.Message} @{ex.HttpMethod.Method}('{ex.Uri.AbsoluteUri}') Content: '{ex.Content}'", ex)
    {
    }
}