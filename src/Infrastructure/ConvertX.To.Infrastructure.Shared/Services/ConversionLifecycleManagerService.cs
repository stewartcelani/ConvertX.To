using ConvertX.To.Application.Domain.Settings;
using ConvertX.To.Application.Interfaces;

namespace ConvertX.To.Infrastructure.Shared.Services;

public class ConversionLifecycleManagerService : IConversionLifecycleManagerService
{
    private readonly ConversionLifecycleManagerSettings _conversionLifecycleManagerSettings;
    private readonly IConversionService _conversionService;
    private readonly IConversionStorageService _conversionStorageService;
    private readonly ILoggerAdapter<ConversionLifecycleManagerService> _logger;

    public ConversionLifecycleManagerService(ILoggerAdapter<ConversionLifecycleManagerService> logger,
        IConversionService conversionService, IConversionStorageService conversionStorageService,
        ConversionLifecycleManagerSettings conversionLifecycleManagerSettings)
    {
        _logger = logger;
        _conversionService = conversionService;
        _conversionStorageService = conversionStorageService;
        _conversionLifecycleManagerSettings = conversionLifecycleManagerSettings;
    }

    public async Task ExpireConversionsAndCleanUpTemporaryStorage()
    {
        var timeToLive =
            DateTimeOffset.Now.Subtract(TimeSpan.FromMinutes(_conversionLifecycleManagerSettings.TimeToLiveInMinutes));
        await _conversionService.ExpireConversions(timeToLive);
        await CleanUpTemporaryStorage();
    }

    private async Task CleanUpTemporaryStorage()
    {
        var nonExpiredConversions = await _conversionService.GetAsync();
        var nonExpiredConversionIds = nonExpiredConversions.Select(x => x.Id.ToString()).ToArray();
        var rootDirectory = _conversionStorageService.GetRootDirectory();
        foreach (var directoryInfo in rootDirectory.GetDirectories()
                     .Where(x => !nonExpiredConversionIds.Contains(x.Name)))
        {
            _logger.LogDebug("Cleaning up expired conversion {conversionId}", directoryInfo.Name);
            _conversionStorageService.DeleteConvertedFile(Guid.Parse(directoryInfo.Name));
        }
    }
}