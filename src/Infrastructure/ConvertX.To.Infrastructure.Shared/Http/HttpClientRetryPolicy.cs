using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace ConvertX.To.Infrastructure.Shared.Http;

public static class HttpClientRetryPolicy
{
    public static AsyncRetryPolicy GetPolicy(ILogger logger, int retryAttempts, int initialRetryDelayInSeconds)
    {
        return Policy
            .Handle<HttpRequestException>( ex => (int)ex.StatusCode! >=500)
            .WaitAndRetryAsync(retryAttempts,
                retryAttempt => TimeSpan.FromSeconds(retryAttempt * initialRetryDelayInSeconds),
                onRetry: (exception, sleepDuration, retryCount, context) =>
                {
                    var message =
                        $"Transient error. Retry {retryCount}/{retryAttempts} in {sleepDuration.Seconds} seconds.";
                    logger?.LogWarning(exception, message);
                });
    }
}