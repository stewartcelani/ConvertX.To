using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace ConvertX.To.Infrastructure.Http;

public static class HttpClientRetryPolicy
{
    public static AsyncRetryPolicy GetPolicy(ILogger logger, int retryAttempts, int initialRetryDelayInSeconds)
    {
        return Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(retryAttempts,
                retryAttempt => TimeSpan.FromSeconds(retryAttempt * initialRetryDelayInSeconds),
                onRetry: (exception, sleepDuration, retryCount, context) =>
                {
                    var message = "Error communicating with destination." +
                                  "Expecting this to be a transient error. " +
                                  $"Retry {retryCount}/4 in {sleepDuration.Seconds} seconds.";
                    logger?.LogWarning(exception, message);
                });
    }
}