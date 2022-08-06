namespace ConvertX.To.ConsoleUI.API.Exceptions;

public class UnsuccessfulStatusCodeException : ApiClientExceptionBase
{
    public UnsuccessfulStatusCodeException()
    {
    }

    public UnsuccessfulStatusCodeException(string message) : base(message)
    {
    }

    public UnsuccessfulStatusCodeException(string message, Exception inner) : base(message, inner)
    {
    }

    public UnsuccessfulStatusCodeException(HttpResponseMessage response) : base(response.ToString())
    {
        HttpResponseMessage = response ?? throw new NullReferenceException(nameof(response));
    }

    public HttpResponseMessage HttpResponseMessage { get; }
}