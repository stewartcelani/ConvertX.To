using ConvertX.To.Application.Interfaces;
using Mapster;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.Application.Converters;

public class ConversionEngine : IConversionEngine
{
    private readonly ILogger<ConversionEngine> _logger;
    private readonly IConverterFactory _converterFactory;
    private readonly IFileService _fileService;

    public ConversionEngine(ILogger<ConversionEngine> logger,
        IConverterFactory converterFactory, IFileService fileService)
    {
        _logger = logger;
        _converterFactory = converterFactory;
        _fileService = fileService;
    }

    public async Task<ConversionResult> ConvertAsync(ConversionTask task)
    {
        _logger.LogInformation(
            $"Processing new conversion request to convert {task.SourceFilePath} to {task.TargetFormat}");

        var converter = _converterFactory.Create(task.SourceFormat, task.TargetFormat);
        
        var convertedFileStream = await converter.ConvertAsync(task.SourceFilePath);

        var convertedFile = await _fileService.SaveFileAsync(task.DirectoryName,
            $"{task.FileNameWithoutExtension}.{task.TargetFormat}", convertedFileStream);

        var response = task.Adapt<ConversionResult>();
        response.ConvertedFileName = convertedFile.Name;
        response.ConvertedFilePath = convertedFile.FullName;
        response.RequestCompleteDate = DateTimeOffset.Now;

        return response;
    }
}