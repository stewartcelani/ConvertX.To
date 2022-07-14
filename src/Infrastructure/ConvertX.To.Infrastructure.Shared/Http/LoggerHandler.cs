using Microsoft.Extensions.Logging;

namespace ConvertX.To.Infrastructure.Http;

class LoggerHandler: DelegatingHandler
{
    private readonly ILogger<LoggerHandler> _logger;

    public LoggerHandler(ILogger<LoggerHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await base.SendAsync(request, cancellationToken);
            _logger.LogInformation(response.StatusCode.ToString());
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Request has failed after several retries");
            throw;
        }
    }
}