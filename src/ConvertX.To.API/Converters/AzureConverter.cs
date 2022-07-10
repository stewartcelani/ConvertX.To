using ConvertX.To.API.Services;

namespace ConvertX.To.API.Converters;

public abstract class AzureConverter : IConverter
{
    private readonly string _sourceFormat;
    private readonly string _targetFormat;
    protected readonly IAzureFileService _azureFileService;
    protected readonly ILogger _logger;
    
    protected AzureConverter(string sourceFormat, string targetFormat, IAzureFileService azureFileService, ILogger logger)
    {
        _sourceFormat = sourceFormat;
        _targetFormat = targetFormat;
        _azureFileService = azureFileService;
        _logger = logger;
    }

    public virtual async Task<Stream> ConvertAsync(string filePath)
    {
        _logger.LogDebug($"{nameof(AzureConverter)}.{nameof(ConvertAsync)}");
        var fileId = await _azureFileService.UploadFileAsync(filePath);
        var convertedFileStream = await _azureFileService.GetConvertedFileAsync(fileId, _targetFormat);
        await _azureFileService.DeleteFileAsync(fileId);
        return convertedFileStream;
    }
}