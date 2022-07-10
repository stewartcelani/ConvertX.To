using ConvertX.To.API.Entities;
using ConvertX.To.API.Services;
using ConvertX.To.API.Settings;
using Mapster;

namespace ConvertX.To.API.Converters;

public class ConversionEngine : IConversionEngine
{
    private readonly ILogger _logger;
    private readonly IConverterFactory _converterFactory;
    private readonly ILocalFileService _localFileService;

    public ConversionEngine(ILogger logger,
        IConverterFactory converterFactory, ILocalFileService localFileService)
    {
        _logger = logger;
        _converterFactory = converterFactory;
        _localFileService = localFileService;
    }

    public async Task<ConversionResult> ConvertAsync(ConversionTask task)
    {
        _logger.LogInformation(
            $"Processing new conversion request to convert {task.SourceFilePath} to {task.TargetFormat}");

        var converter = _converterFactory.Create(task.SourceFormat, task.TargetFormat);
        
        var convertedFileStream = await converter.ConvertAsync(task.SourceFilePath);

        var convertedFile = await _localFileService.SaveFileAsync(task.DirectoryName,
            $"{task.FileNameWithoutExtension}.{task.TargetFormat}", convertedFileStream);

        var response = task.Adapt<ConversionResult>();
        response.ConvertedFileName = convertedFile.Name;
        response.ConvertedFilePath = convertedFile.FullName;
        response.RequestCompleteDate = DateTimeOffset.Now;

        return response;
    }
}