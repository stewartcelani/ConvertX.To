using ConvertX.To.API.Settings;

namespace ConvertX.To.API.Converters;

public class ConversionEngine : IConversionEngine
{
    private readonly ILogger _logger;
    private readonly IConverterFactory _converterFactory;

    public ConversionEngine(ILogger logger,
        IConverterFactory converterFactory)
    {
        _logger = logger;
        _converterFactory = converterFactory;
    }

    public async Task<FileInfo> ConvertAsync(string sourceFormat, string targetFormat, FileInfo sourceFile)
    {
        _logger.LogInformation("Starting conversion.");

        var converter = _converterFactory.Create(sourceFormat, targetFormat);
        
        var convertedFile = await converter.ConvertAsync(sourceFile);

        return convertedFile;
    }
}