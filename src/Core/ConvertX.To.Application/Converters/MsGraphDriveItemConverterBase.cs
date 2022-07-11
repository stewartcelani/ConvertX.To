using ConvertX.To.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.Application.Converters;

public abstract class MsGraphDriveItemConverterBase : IConverter
{
    private readonly string _targetFormat;
    private readonly IMsGraphFileConversionService _msGraphFileConversionService;
    private readonly ILogger _logger;
    
    protected MsGraphDriveItemConverterBase(string targetFormat, IMsGraphFileConversionService msGraphFileConversionService, ILogger logger)
    {
        _targetFormat = targetFormat;
        _msGraphFileConversionService = msGraphFileConversionService;
        _logger = logger;
    }

    public virtual async Task<Stream> ConvertAsync(string filePath)
    {
        var fileId = await _msGraphFileConversionService.UploadFileAsync(filePath);
        var convertedFileStream = await _msGraphFileConversionService.GetFileInTargetFormatAsync(fileId, _targetFormat);
        await _msGraphFileConversionService.DeleteFileAsync(fileId);
        return convertedFileStream;
    }
}