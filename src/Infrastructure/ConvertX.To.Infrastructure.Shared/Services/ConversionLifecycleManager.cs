using ConvertX.To.Application.Interfaces;
using ConvertX.To.Domain.Settings;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.Infrastructure.Shared.Services;

public class ConversionLifecycleManager : IConversionLifecycleManager
{
    private readonly ILogger<ConversionLifecycleManager> _logger;
    private readonly IConversionService _conversionService;
    private readonly IFileService _fileService;
    private readonly ConversionLifecycleManagerSettings _conversionLifecycleManagerSettings;

    public ConversionLifecycleManager(ILogger<ConversionLifecycleManager> logger, IConversionService conversionService, IFileService fileService, ConversionLifecycleManagerSettings conversionLifecycleManagerSettings)
    {
        _logger = logger;
        _conversionService = conversionService;
        _fileService = fileService;
        _conversionLifecycleManagerSettings = conversionLifecycleManagerSettings;
    }

    public async Task ExpireConversions()
    {
        _logger.LogTrace("{Class}.{Method}", nameof(ConversionLifecycleManager), nameof(ExpireConversions));
        await _conversionService.ExpireConversions(_conversionLifecycleManagerSettings.TimeToLiveInMinutes);
    }

    public void CleanUpTemporaryStorage()
    {
        _logger.LogTrace("{Class}.{Method}", nameof(ConversionLifecycleManager), nameof(CleanUpTemporaryStorage));
        var nonExpiredConversionIds = _conversionService.GetAllIds().Select(x => x.ToString()).ToList();
        var rootDirectory = _fileService.GetRootDirectory();
        foreach (var directoryInfo in rootDirectory.GetDirectories().Where(x => !nonExpiredConversionIds.Contains(x.Name)))
        {
            _logger.LogDebug("Cleaning up expired conversion {conversionId}", directoryInfo.Name);
            _fileService.DeleteDirectory(directoryInfo.Name);
        }
    }

    public async Task ExpireConversionsAndCleanUpTemporaryStorage()
    {
        _logger.LogTrace("{Class}.{Method}", nameof(ConversionLifecycleManager), nameof(ExpireConversionsAndCleanUpTemporaryStorage));
        await ExpireConversions();
        CleanUpTemporaryStorage();
    }
}