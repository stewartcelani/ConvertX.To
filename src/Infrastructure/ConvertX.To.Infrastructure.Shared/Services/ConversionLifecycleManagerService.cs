using ConvertX.To.Application.Domain.Settings;
using ConvertX.To.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.Infrastructure.Shared.Services;

public class ConversionLifecycleManagerService : IConversionLifecycleManagerService
{
    private readonly ILogger<ConversionLifecycleManagerService> _logger;
    private readonly IConversionService _conversionService;
    private readonly IConversionStorageService _conversionStorageService;
    private readonly ConversionLifecycleManagerSettings _conversionLifecycleManagerSettings;

    public ConversionLifecycleManagerService(ILogger<ConversionLifecycleManagerService> logger,
        IConversionService conversionService, IConversionStorageService conversionStorageService,
        ConversionLifecycleManagerSettings conversionLifecycleManagerSettings)
    {
        _logger = logger;
        _conversionService = conversionService;
        _conversionStorageService = conversionStorageService;
        _conversionLifecycleManagerSettings = conversionLifecycleManagerSettings;
    }

    public async Task ExpireConversions()
    {
        _logger.LogTrace("{Class}.{Method}", nameof(ConversionLifecycleManagerService), nameof(ExpireConversions));
        await _conversionService.ExpireConversions(_conversionLifecycleManagerSettings.TimeToLiveInMinutes);
    }

    public async Task CleanUpTemporaryStorage()
    {
        _logger.LogTrace("{Class}.{Method}", nameof(ConversionLifecycleManagerService),
            nameof(CleanUpTemporaryStorage));
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

    public async Task ExpireConversionsAndCleanUpTemporaryStorage()
    {
        _logger.LogTrace("{Class}.{Method}", nameof(ConversionLifecycleManagerService),
            nameof(ExpireConversionsAndCleanUpTemporaryStorage));
        await ExpireConversions();
        await CleanUpTemporaryStorage();
    }
}