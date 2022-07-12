using ConvertX.To.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.Application.Converters;

public abstract class MsGraphDriveItemConverterBase : IConverter
{
    private readonly string _sourceFormat;
    private readonly string _targetFormat;
    private readonly IMsGraphFileConversionService _msGraphFileConversionService;
    private readonly ILogger _logger;
    
    protected MsGraphDriveItemConverterBase(string sourceFormat, string targetFormat, ILogger logger, IMsGraphFileConversionService msGraphFileConversionService)
    {
        _sourceFormat = sourceFormat;
        _targetFormat = targetFormat;
        _msGraphFileConversionService = msGraphFileConversionService;
        _logger = logger;
    }

    public virtual async Task<Stream> ConvertAsync(Stream source)
    {
        var fileId = await _msGraphFileConversionService.UploadFileAsync(_sourceFormat, source);
        var convertedStream = await _msGraphFileConversionService.GetFileInTargetFormatAsync(fileId, _targetFormat);
        await _msGraphFileConversionService.DeleteFileAsync(fileId);
        return convertedStream;
    }
}