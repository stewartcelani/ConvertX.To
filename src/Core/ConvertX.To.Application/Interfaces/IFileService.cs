namespace ConvertX.To.Application.Interfaces;

public interface IFileService
{
    Task SaveFileAsync(string path, Stream stream);
    void DeleteDirectory(string path);
    Stream GetFile(string path);
}