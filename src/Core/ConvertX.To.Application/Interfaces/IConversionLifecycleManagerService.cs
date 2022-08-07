namespace ConvertX.To.Application.Interfaces;

public interface IConversionLifecycleManagerService
{
    Task ExpireConversionsAndCleanUpTemporaryStorage();
}