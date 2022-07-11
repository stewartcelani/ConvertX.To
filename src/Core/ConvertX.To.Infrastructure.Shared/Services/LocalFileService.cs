using ConvertX.To.Application.Interfaces;
using ConvertX.To.Domain.Settings;

namespace ConvertX.To.Infrastructure.Shared.Services;

public class LocalFileService : IFileService
{
    private readonly FileServiceSettings _fileServiceSettings;

    public LocalFileService(FileServiceSettings fileServiceSettings)
    {
        _fileServiceSettings = fileServiceSettings;
        EnsureDirectory(_fileServiceSettings.RootDirectory);
    }
    
    public async Task<FileInfo> SaveFileAsync(string relativeDirectoryPath, string fileName, Stream stream)
    {
        EnsureDirectory(Path.Combine(_fileServiceSettings.RootDirectory, relativeDirectoryPath));
        var filePath = Path.Combine(_fileServiceSettings.RootDirectory, relativeDirectoryPath, fileName);
        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await stream.CopyToAsync(fileStream);
        await stream.DisposeAsync();
        return new FileInfo(filePath);
    }

    public Stream GetStream(string relativeDirectoryPath, string fileName) => File.Open(Path.Combine(_fileServiceSettings.RootDirectory, relativeDirectoryPath, fileName), FileMode.Open);

    private static void EnsureDirectory(string directoryPath)
    {
        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
    }
}