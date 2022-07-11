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
        // TODO: To download Jpg files need to pass height and width so need an optional query params to pass to GetFileInTargetFormatAsync
        var convertedFileStream = await _msGraphFileConversionService.GetFileInTargetFormatAsync(fileId, _targetFormat);
        await _msGraphFileConversionService.DeleteFileAsync(fileId);
        return convertedFileStream;
    }
}