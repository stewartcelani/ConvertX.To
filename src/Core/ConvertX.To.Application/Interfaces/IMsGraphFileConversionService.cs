namespace ConvertX.To.Application.Interfaces;

public interface IMsGraphFileConversionService
{
    Task<string> UploadFileAsync(string filePath);
    Task<Stream> GetFileInTargetFormatAsync(string fileId, string targetFormat);
    Task DeleteFileAsync(string fileId);
}