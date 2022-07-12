namespace ConvertX.To.Application.Interfaces;

public interface IStorageService
{
    Task SaveFileAsync(string conversionId, string fileName, Stream stream);
    Stream GetFileAsync(string conversionId, string fileName);

}