using Polly;
using Polly.Retry;

namespace ConvertX.To.Infrastructure.Http;

public static class HttpClientRetryPolicy
{
    public static AsyncRetryPolicy GetPolicy()
    {
        return Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromSeconds(retryAttempt * 1));
    }
}