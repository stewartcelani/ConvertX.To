namespace ConvertX.To.Application.Interfaces;

public interface IConversionLifecycleManager
{
    Task ExpireConversions();
    void CleanUpTemporaryStorage();
    Task ExpireConversionsAndCleanUpTemporaryStorage();
}