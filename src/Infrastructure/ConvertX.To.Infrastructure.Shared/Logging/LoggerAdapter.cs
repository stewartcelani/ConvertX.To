using ConvertX.To.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.Infrastructure.Shared.Logging;


public class LoggerAdapter<TType> : ILoggerAdapter<TType>
{
    private readonly ILogger<TType> _logger;

    public LoggerAdapter(ILogger<TType> logger)
    {
        _logger = logger;
    }

    public void LogTrace(string? message, params object?[] args)
    {
        _logger.LogTrace(message, args);
    }

    public void LogDebug(string? message, params object?[] args)
    {
        _logger.LogDebug(message, args);
    }

    public void LogInformation(string? message, params object?[] args)
    {
        _logger.LogInformation(message, args);
    }

    public void LogWarning(string? message, params object?[] args)
    {
        _logger.LogWarning(message, args);
    }

    public void LogError(Exception? exception, string? message, params object?[] args)
    {
        _logger.LogError(exception, message, args);
    }

    public void LogCritical(Exception exception, string? message, params object?[] args)
    {
        _logger.LogCritical(exception, message, args);
    }
}