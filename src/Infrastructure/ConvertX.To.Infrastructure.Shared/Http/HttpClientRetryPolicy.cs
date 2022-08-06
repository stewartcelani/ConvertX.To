using ConvertX.To.Application.Interfaces;
using Polly;
using Polly.Retry;

namespace ConvertX.To.Infrastructure.Shared.Http;

public static class HttpClientRetryPolicy
{
    public static AsyncRetryPolicy GetPolicy(ILoggerAdapter logger, int retryAttempts, int initialRetryDelayInSeconds)
    {
        return Policy
            .Handle<HttpRequestException>(ex => { return (int)ex.StatusCode! >= 500; })
            .WaitAndRetryAsync(retryAttempts,
                retryAttempt => TimeSpan.FromSeconds(retryAttempt * initialRetryDelayInSeconds),
                (exception, sleepDuration, retryCount, context) =>
                {
                    var message =
                        $"Transient error. Retry {retryCount}/{retryAttempts} in {sleepDuration.Seconds} seconds.";
                    logger?.LogError(exception, message);
                });
    }
}