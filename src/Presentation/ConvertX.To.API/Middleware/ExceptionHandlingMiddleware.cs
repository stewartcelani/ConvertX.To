using System.Diagnostics.CodeAnalysis;
using ConvertX.To.Application.Interfaces;

namespace ConvertX.To.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly ILoggerAdapter<ExceptionHandlingMiddleware> _logger;
    private readonly RequestDelegate _request;

    public ExceptionHandlingMiddleware(RequestDelegate request, ILoggerAdapter<ExceptionHandlingMiddleware> logger)
    {
        _request = request;
        _logger = logger;
    }

    [ExcludeFromCodeCoverage]
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _request(context);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex.Message);
            context.Response.StatusCode = 500;
            await context.Response.CompleteAsync();
        }
    }
}