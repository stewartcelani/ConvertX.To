using ConvertX.To.Application.Interfaces;
using ConvertX.To.Domain.Settings;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.Infrastructure.Shared.Services;

public class ConversionLifecycleManagerService : IConversionLifecycleManagerService
{
    private readonly ILogger<ConversionLifecycleManagerService> _logger;
    private readonly IConversionService _conversionService;
    private readonly IFileService _fileService;
    private readonly ConversionLifecycleManagerSettings _conversionLifecycleManagerSettings;

    public ConversionLifecycleManagerService(ILogger<ConversionLifecycleManagerService> logger,
        IConversionService conversionService, IFileService fileService,
        ConversionLifecycleManagerSettings conversionLifecycleManagerSettings)
    {
        _logger = logger;
        _conversionService = conversionService;
        _fileService = fileService;
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
        var nonExpiredConversionIds = (await _conversionService.GetAllIdsAsync()).Select(x => x.ToString()).ToArray();
        var rootDirectory = _fileService.GetRootDirectory();
        foreach (var directoryInfo in rootDirectory.GetDirectories()
                     .Where(x => !nonExpiredConversionIds.Contains(x.Name)))
        {
            _logger.LogDebug("Cleaning up expired conversion {conversionId}", directoryInfo.Name);
            _fileService.DeleteDirectory(directoryInfo.Name);
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