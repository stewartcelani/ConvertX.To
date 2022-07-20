namespace ConvertX.To.Application.Exceptions;

public class HttpResponseException : ConvertXToExceptionBase, IBusinessException
{
    public HttpResponseMessage HttpResponseMessage { get; }

    protected HttpResponseException(string message, HttpResponseMessage httpResponseMessage) : base(message)
    {
        HttpResponseMessage = httpResponseMessage ?? throw new NullReferenceException(nameof(httpResponseMessage));
    }
    
    protected HttpResponseException(string message, HttpResponseMessage httpResponseMessage, Exception inner) : base(message, inner)
    {
        HttpResponseMessage = httpResponseMessage ?? throw new NullReferenceException(nameof(httpResponseMessage));
    }
    
}