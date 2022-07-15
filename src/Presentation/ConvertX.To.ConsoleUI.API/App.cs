using ConvertX.To.API.Contracts.V1.Responses;
using ConvertX.To.ConsoleUI.API.ApiClient;
using ConvertX.To.ConsoleUI.API.Exceptions;
using Microsoft.Extensions.Logging;
using MimeTypes.Core;
using Refit;

namespace ConvertX.To.ConsoleUI.API;

public class App
{
    private readonly ILogger<App> _logger;
    private readonly IApiClient _apiClient;

    private DirectoryInfo _directory = new DirectoryInfo(@"C:\dev\convertx.to\sample_files\");

    public App(ILogger<App> logger, IApiClient apiClient)
    {
        _logger = logger;
        _apiClient = apiClient;
    }
    
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("{Class}.{Method}", nameof(App), nameof(RunAsync));

        var supportedConversionsResponse = await _apiClient.GetSupportedConversionsAsync();

        var supportedConversions = supportedConversionsResponse.Content!;

        var files = _directory.GetFiles("*.*", SearchOption.AllDirectories).Where(x => !x.Name.Contains("ConvertX.To"))
            .ToList();

        var parallelOptions = new ParallelOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = 8
        };

        // TODO: Look into Refit CancellationToken's and how to pass them through all the way
        await Parallel.ForEachAsync(files, parallelOptions, async (file, ctx) =>
        {
            await ConvertFileToAllSupportedFormatsAsync(file, supportedConversions);
        });
    }

    private async Task ConvertFileToAllSupportedFormatsAsync(FileInfo file,
        SupportedConversionsResponse supportedConversions)
    {
        var sourceFormat = file.Extension.Replace(".", "");
        if (!supportedConversions.SourceFormatTo.ContainsKey(sourceFormat)) return;
        
        var supportedTargetFormats = supportedConversions.SourceFormatTo[sourceFormat];
        
        foreach (var targetFormat in supportedTargetFormats)
        {
            try
            {
                await ConvertFileToTargetFormat(file, targetFormat);
            }
            catch (UnsuccessfulStatusCodeException ex)
            {
                _logger.LogError("{statusCode} '{reasonPhrase}' error from API while attempting to convert {fileName} to {targetFormat}. Skipping file...", (int)ex.HttpResponseMessage.StatusCode, ex.HttpResponseMessage.ReasonPhrase, file.Name, targetFormat);
            }
        }
    }

    private async Task ConvertFileToTargetFormat(FileInfo fileInfo, string targetFormat)
    {
        if (fileInfo.Length == 0) return;

        _logger.LogInformation("Converting {fileName} to {targetFormat}", fileInfo.Name, targetFormat);

        var conversionResponse = await _apiClient.ConvertAsync(targetFormat,
            new StreamPart(fileInfo.OpenRead(), fileInfo.Name, MimeTypeMap.GetMimeType(fileInfo.Extension)));

        var conversion = conversionResponse.Content!;

        var downloadConvertedFileAsyncResponse = await _apiClient.DownloadConvertedFileAsync(conversion.Id);

        await using var convertedStream = await downloadConvertedFileAsyncResponse.Content!.ReadAsStreamAsync();

        await SaveFileAsync(Path.Combine(fileInfo.DirectoryName!, GetConvertedFileName(conversion)), convertedStream);

        _logger.LogInformation("{fileName} successfully converted to {targetFormat}!", fileInfo.Name, targetFormat);
    }

    private static async Task SaveFileAsync(string path, Stream convertedStream)
    {
        await using var fileStream = new FileStream(path, FileMode.Create);
        await convertedStream.CopyToAsync(fileStream);
        await convertedStream.DisposeAsync();
        await fileStream.DisposeAsync();
    }

    private static string GetConvertedFileName(ConversionResponse conversion)
    {
        return conversion.TargetFormat == conversion.ConvertedFileExtension
            ? $"{conversion.SourceFileName}.ConvertX.To.{conversion.TargetFormat}"
            : $"{conversion.SourceFileName}.ConvertX.To.{conversion.TargetFormat}.{conversion.ConvertedFileExtension}";
    }
}