using ConvertX.To.Application.Interfaces;
using ConvertX.To.Domain.Settings;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.Infrastructure.Persistence.Cron;

public class ExpireAndCleanUpConversions
{
    private readonly ILogger<ExpireAndCleanUpConversions> _logger;
    private readonly IConversionService _conversionService;
    private readonly IFileService _fileService;
    private readonly TimeToLiveSettings _timeToLiveSettings;
    
    public ExpireAndCleanUpConversions(ILogger<ExpireAndCleanUpConversions> logger, IConversionService conversionService, IFileService fileService, TimeToLiveSettings timeToLiveSettings)
    {
        _logger = logger;
        _conversionService = conversionService;
        _fileService = fileService;
        _timeToLiveSettings = timeToLiveSettings;
    }

    public async Task RunAsync()
    {
        _logger.LogInformation("Hello from {className}", nameof(ExpireAndCleanUpConversions));
    }
}