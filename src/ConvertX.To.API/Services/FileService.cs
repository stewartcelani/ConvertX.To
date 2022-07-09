using ConvertX.To.API.Exceptions;
using ConvertX.To.API.Settings;

namespace ConvertX.To.API.Services;

public class FileService : IFileService
{
    private readonly ILogger<FileService> _logger;
    private readonly AzureSettings _azureSettings;

    public FileService(ILogger<FileService> logger, AzureSettings azureSettings)
    {
        _logger = logger;
        _azureSettings = azureSettings;
    }

    public async Task<FileInfo> SaveFile(string directoryPath, IFormFile file)
    {
        EnsureDirectory(directoryPath);
        if (file.Length == 0)
            throw new InvalidFileLengthException("Uploaded file length must be greater than zero.");
        var filePath = Path.Combine(directoryPath, file.FileName);
        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(fileStream);
        return new FileInfo(filePath);
    }

    public Stream GetStream(string filePath)
    {
        Stream stream = File.Open(filePath, FileMode.Open);
        return stream;
    }

    private void EnsureDirectory(string directoryPath)
    {
        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
    }
}