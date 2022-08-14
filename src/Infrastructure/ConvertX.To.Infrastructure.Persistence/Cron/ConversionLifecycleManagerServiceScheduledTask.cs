using System.Diagnostics.CodeAnalysis;
using ConvertX.To.Application.Interfaces;

namespace ConvertX.To.Infrastructure.Persistence.Cron;

[ExcludeFromCodeCoverage]
public class ConversionLifecycleManagerServiceScheduledTask
{
    private readonly IConversionLifecycleManagerService _conversionLifecycleManagerService;

    public ConversionLifecycleManagerServiceScheduledTask(
        IConversionLifecycleManagerService conversionLifecycleManagerService)
    {
        _conversionLifecycleManagerService = conversionLifecycleManagerService;
    }

    public async Task RunAsync()
    {
        await _conversionLifecycleManagerService.ExpireConversionsAndCleanUpTemporaryStorage();
    }
}