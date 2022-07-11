namespace ConvertX.To.Application.Interfaces;

public interface IGraphFileService
{
    Task<string> UploadFileAsync(string filePath);
    Task<Stream> GetConvertedFileAsync(string fileId, string targetFormat);
    Task DeleteFileAsync(string fileId);
}