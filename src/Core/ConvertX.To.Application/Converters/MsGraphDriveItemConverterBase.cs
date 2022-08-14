using ConvertX.To.Application.Interfaces;
using ConvertX.To.Domain.Options;

namespace ConvertX.To.Application.Converters;

public abstract class MsGraphDriveItemConverterBase : IConverter
{
    private readonly ILoggerAdapter<IConverter> _logger;
    private readonly IMsGraphFileConversionService _msGraphFileConversionService;
    protected readonly string _sourceFormat;
    protected readonly string _targetFormat;

    protected MsGraphDriveItemConverterBase(string sourceFormat, string targetFormat, ILoggerAdapter<IConverter> logger,
        IMsGraphFileConversionService msGraphFileConversionService)
    {
        _sourceFormat = sourceFormat;
        _targetFormat = targetFormat;
        _msGraphFileConversionService = msGraphFileConversionService;
        _logger = logger;
    }

    public virtual async Task<(string, Stream)> ConvertAsync(Stream source, ConversionOptions conversionOptions)
    {
        var fileId = await _msGraphFileConversionService.UploadFileAsync(_sourceFormat, source);
        var convertedStream = await _msGraphFileConversionService.GetFileInTargetFormatAsync(fileId, _targetFormat);
        await _msGraphFileConversionService.DeleteFileAsync(fileId);
        return (_targetFormat, convertedStream);
    }
}