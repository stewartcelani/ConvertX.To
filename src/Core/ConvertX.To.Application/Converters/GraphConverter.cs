using ConvertX.To.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.Application.Converters;

public abstract class GraphConverter : IConverter
{
    private readonly string _sourceFormat;
    private readonly string _targetFormat;
    protected readonly IGraphFileService _graphFileService;
    protected readonly ILogger _logger;
    
    protected GraphConverter(string sourceFormat, string targetFormat, IGraphFileService graphFileService, ILogger logger)
    {
        _sourceFormat = sourceFormat;
        _targetFormat = targetFormat;
        _graphFileService = graphFileService;
        _logger = logger;
    }

    public virtual async Task<Stream> ConvertAsync(string filePath)
    {
        _logger.LogDebug($"{nameof(GraphConverter)}.{nameof(ConvertAsync)}");
        var fileId = await _graphFileService.UploadFileAsync(filePath);
        var convertedFileStream = await _graphFileService.GetConvertedFileAsync(fileId, _targetFormat);
        await _graphFileService.DeleteFileAsync(fileId);
        return convertedFileStream;
    }
}