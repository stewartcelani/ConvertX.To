namespace ConvertX.To.API.Services;

public interface IAzureFileService
{
    Task<string> UploadFileAsync(string filePath);
    Task<Stream> GetConvertedFileAsync(string fileId, string targetFormat);
    Task DeleteFileAsync(string fileId);
}