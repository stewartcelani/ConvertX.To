namespace ConvertX.To.Application.Interfaces;

public interface IFileService
{
    Task<FileInfo> SaveFileAsync(string relativeDirectoryPath, string fileName, Stream stream);
    Stream GetStream(string relativeDirectoryPath, string fileName);

}