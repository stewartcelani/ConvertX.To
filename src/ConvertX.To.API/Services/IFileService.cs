namespace ConvertX.To.API.Services;

public interface IFileService
{
    Task<FileInfo> SaveFile(string directoryPath, IFormFile file);
    Stream GetStream(string filePath);
}