using ConvertX.To.API.Converters;
using ConvertX.To.API.Entities;

namespace ConvertX.To.API.Services;

public class ConversionService : IConversionService
{
    private readonly ILogger<ConversionService> _logger;
    private readonly IFileService _fileService;
    private readonly IConversionEngine _conversionEngine;

    public ConversionService(IConversionEngine conversionEngine, ILogger<ConversionService> logger, IFileService fileService)
    {
        _conversionEngine = conversionEngine;
        _logger = logger;
        _fileService = fileService;
    }

    public async Task<FileInfo> ConvertAsync(string sourceFormat, string targetFormat, IFormFile formFile)
    {
        _logger.LogDebug(nameof(ConversionService));

        var directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sourceFile = await _fileService.SaveFile(directory, formFile);
        var convertedFile = await _conversionEngine.ConvertAsync(sourceFormat, targetFormat, sourceFile);
        return convertedFile;

        // TODO: This needs to return a ConversionResult with just GUID after storing the conversion in a database
        /*return new ConversionResult
        {
            FileId = Guid.NewGuid()
        };*/
    }
}