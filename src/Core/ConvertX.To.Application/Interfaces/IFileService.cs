namespace ConvertX.To.Application.Interfaces;

public interface IFileService
{
    DirectoryInfo GetDirectory(string path);
    DirectoryInfo GetRootDirectory();
    void DeleteDirectory(string path);
    Stream GetFile(string path);
    Task SaveFileAsync(string path, Stream stream);
}