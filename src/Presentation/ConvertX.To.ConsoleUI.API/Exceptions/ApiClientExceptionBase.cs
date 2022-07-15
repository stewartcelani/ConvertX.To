using Refit;

namespace ConvertX.To.ConsoleUI.API.Exceptions;

public abstract class ApiClientExceptionBase : Exception
{
    
    protected ApiClientExceptionBase() { }

    protected ApiClientExceptionBase(string message)
    {
    }

    protected ApiClientExceptionBase(string message, Exception inner)
    {
    }
    
}