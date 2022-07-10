using ConvertX.To.API.Exceptions;
using ConvertX.To.API.Exceptions.Business;
using ConvertX.To.API.Settings;

namespace ConvertX.To.API.Services;

public class LocalFileService : ILocalFileService
{
    private readonly LocalFileServiceSettings _localFileServiceSettings;

    public LocalFileService(LocalFileServiceSettings localFileServiceSettings)
    {
        _localFileServiceSettings = localFileServiceSettings;
        EnsureDirectory(_localFileServiceSettings.RootDirectory);
    }

    public async Task<FileInfo> SaveFileAsync(string relativeDirectoryPath, IFormFile file)
    {
        EnsureDirectory(Path.Combine(_localFileServiceSettings.RootDirectory, relativeDirectoryPath));
        if (file.Length == 0)
            throw new InvalidFileLengthException("Uploaded file length must be greater than zero.");
        var filePath = Path.Combine(_localFileServiceSettings.RootDirectory, relativeDirectoryPath, file.FileName);
        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(fileStream);
        return new FileInfo(filePath);
    }

    public async Task<FileInfo> SaveFileAsync(string relativeDirectoryPath, string fileName, Stream stream)
    {
        EnsureDirectory(Path.Combine(_localFileServiceSettings.RootDirectory, relativeDirectoryPath));
        var filePath = Path.Combine(_localFileServiceSettings.RootDirectory, relativeDirectoryPath, fileName);
        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await stream.CopyToAsync(fileStream);
        await stream.DisposeAsync();
        return new FileInfo(filePath);
    }

    public Stream GetStream(string relativeDirectoryPath, string fileName) => File.Open(Path.Combine(_localFileServiceSettings.RootDirectory, relativeDirectoryPath, fileName), FileMode.Open);

    private void EnsureDirectory(string directoryPath)
    {
        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
    }
}