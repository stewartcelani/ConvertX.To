using ConvertX.To.Application.Interfaces;

namespace ConvertX.To.Infrastructure.Persistence.Cron;

public class ConversionLifecycleManagerScheduledTask
{
    private readonly IConversionLifecycleManager _conversionLifecycleManager;

    public ConversionLifecycleManagerScheduledTask(IConversionLifecycleManager conversionLifecycleManager)
    {
        _conversionLifecycleManager = conversionLifecycleManager;
    }

    public async Task RunAsync()
    {
        await _conversionLifecycleManager.ExpireConversionsAndCleanUpTemporaryStorage();
    }
}