namespace ConvertX.To.Application.Interfaces;

public interface IMsGraphFileConversionService
{
    Task<string> UploadFileAsync(string sourceFormat, Stream source);
    Task<Stream> GetFileInTargetFormatAsync(string fileId, string targetFormat);
    Task DeleteFileAsync(string fileId);
}