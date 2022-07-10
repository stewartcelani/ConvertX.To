namespace ConvertX.To.API.Services;

public interface ILocalFileService
{
    Task<FileInfo> SaveFileAsync(string relativeDirectoryPath, IFormFile file);
    Task<FileInfo> SaveFileAsync(string relativeDirectoryPath, string fileName, Stream stream);
    Stream GetStream(string relativeDirectoryPath, string fileName);

}