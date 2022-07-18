namespace ConvertX.To.Application.Interfaces;

public interface IConversionLifecycleManagerService
{
    Task ExpireConversions();
    Task CleanUpTemporaryStorage();
    Task ExpireConversionsAndCleanUpTemporaryStorage();
}