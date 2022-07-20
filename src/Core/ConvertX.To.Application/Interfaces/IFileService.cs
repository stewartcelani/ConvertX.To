namespace ConvertX.To.Application.Interfaces;

public interface IFileService
{
    Task SaveFileAsync(string path, Stream stream);
}