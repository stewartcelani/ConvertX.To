using ConvertX.To.Application.Converters;
using ConvertX.To.Application.Domain;
using ConvertX.To.Application.Interfaces;
using ConvertX.To.Domain.Options;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.ConsoleUI.Core;

public class App
{
    private readonly IConversionEngine _conversionEngine;
    private readonly IFileService _fileService;
    private readonly ILogger<App> _logger;

    private readonly DirectoryInfo _directory = new(@"C:\dev\convertx.to\sample_files");

    public App(ILogger<App> logger, IConversionEngine conversionEngine, IFileService fileService)
    {
        _logger = logger;
        _conversionEngine = conversionEngine;
        _fileService = fileService;
    }

    public async Task RunAsync(string[] args, CancellationToken ct = default)
    {
        var supportedConversions = ConversionEngine.GetSupportedConversions();

        var files = _directory
            .GetFiles("*.*", SearchOption.AllDirectories)
            .Where(x => !x.Name.Contains("ConvertX.To"))
            .ToList();

        var parallelOptions = new ParallelOptions
        {
            CancellationToken = ct,
            MaxDegreeOfParallelism = 8
        };

        await Parallel.ForEachAsync(files, parallelOptions,
            async (file, ctx) => { await ConvertFileToAllSupportedFormatsAsync(file, supportedConversions); });
    }

    private async Task ConvertFileToAllSupportedFormatsAsync(FileInfo fileInfo,
        SupportedConversions supportedConversions)
    {
        var sourceFormat = fileInfo.Extension.Replace(".", "");
        if (!supportedConversions.SourceFormatTo.ContainsKey(sourceFormat)) return;
        var supportedTargetFormats = supportedConversions.SourceFormatTo[sourceFormat];
        foreach (var targetFormat in supportedTargetFormats)
        {
            _logger.LogInformation("Converting {fileName} to {targetFormat}", fileInfo.Name, targetFormat);
            var conversionOptions = new ConversionOptions();
            try
            {
                var (convertedFileExtension, convertedStream) =
                    await _conversionEngine.ConvertAsync(sourceFormat, targetFormat, fileInfo.OpenRead(),
                        conversionOptions);
                var fileName = GetConvertedFileName(fileInfo.Name, targetFormat, convertedFileExtension);
                await _fileService.SaveFileAsync(Path.Combine(fileInfo.DirectoryName!, fileName), convertedStream);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error while attempting to convert {fileName} to {targetFormat}. Skipping file...",
                    fileInfo.Name,
                    targetFormat);
            }

            _logger.LogInformation("{fileName} successfully converted to {targetFormat}!", fileInfo.Name, targetFormat);
        }
    }

    private static string GetConvertedFileName(string sourceFileName, string targetFormat,
        string convertedFileExtension)
    {
        var fileName = targetFormat == convertedFileExtension
            ? $"{sourceFileName}.ConvertX.To.{targetFormat}"
            : $"{sourceFileName}.ConvertX.To.{targetFormat}.{convertedFileExtension}";
        return fileName;
    }
}