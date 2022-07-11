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

    // TODO: ConversionEngine should just take in a stream, source format and target format and return a stream
    // The other logic should be happening somewhere else
    // in a controller because if you wanted a ConsoleApp in the future
    public async Task<Stream> ConvertAsync(string sourceFormat, string targetFormat, Stream source)
    {
        _logger.LogInformation(
            $"Processing new conversion request to convert {sourceFormat} to {targetFormat}");

        var converter = _converterFactory.Create(sourceFormat, targetFormat);
        
        return await converter.ConvertAsync(source);
    }
}