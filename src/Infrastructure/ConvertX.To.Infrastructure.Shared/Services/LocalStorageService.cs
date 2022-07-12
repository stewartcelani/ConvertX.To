using ConvertX.To.Application.Interfaces;
using ConvertX.To.Domain.Settings;

namespace ConvertX.To.Infrastructure.Shared.Services;

public class LocalStorageService : IStorageService
{
    private readonly LocalStorageSettings _localStorageSettings;

    public LocalStorageService(LocalStorageSettings localStorageSettings)
    {
        _localStorageSettings = localStorageSettings;
        EnsureDirectory(_localStorageSettings.RootDirectory);
    }
    
    public async Task SaveFileAsync(string conversionId, string fileName, Stream stream)
    {
        EnsureDirectory(Path.Combine(_localStorageSettings.RootDirectory, conversionId));
        var filePath = Path.Combine(_localStorageSettings.RootDirectory, conversionId, fileName);
        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await stream.CopyToAsync(fileStream);
        await stream.DisposeAsync();
    }

    public Stream GetFileAsync(string conversionId, string fileName) => File.Open(BuildPath(conversionId, fileName), FileMode.Open);

    private static void EnsureDirectory(string directoryPath)
    {
        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
    }

    private string BuildPath(string conversionId, string fileName) =>
        Path.Combine(_localStorageSettings.RootDirectory, conversionId, fileName);
}