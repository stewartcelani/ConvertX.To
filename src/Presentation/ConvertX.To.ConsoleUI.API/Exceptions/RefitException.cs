/*using Refit;

namespace ConvertX.To.ConsoleUI.API.Exceptions;

public class RefitException : ApiClientExceptionBase
{
    public ApiException? ApiException { get; }
    
    public RefitException() { }
    public RefitException(string message) : base(message) { }
    public RefitException(string message, Exception inner) : base(message, inner) { }

    public RefitException(ApiException ex) : base($"{ex.Message} @{ex.HttpMethod.Method}('{ex.Uri.AbsoluteUri}') Content: '{ex.Content}'", ex)
    {
        ApiException = ex;
    }
}*/