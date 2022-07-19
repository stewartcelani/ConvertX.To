using ConvertX.To.Application.Converters;
using ConvertX.To.ConsoleUI.API.ApiClient;
using ConvertX.To.ConsoleUI.API.Exceptions;
using ConvertX.To.Domain.Common;
using Microsoft.Extensions.Logging;
using Mapster;
using MimeTypes.Core;
using Refit;

namespace ConvertX.To.ConsoleUI.API.Services;

public class ConversionService : IConversionService
{
    private readonly ILogger<ConversionService> _logger;
    private readonly IApiClient _apiClient;

    public ConversionService(ILogger<ConversionService> logger, IApiClient apiClient)
    {
        _logger = logger;
        _apiClient = apiClient;
    }


    public async Task<SupportedConversions> GetSupportedConversionsAsync()
    {
        var supportedConversionsResponse = await _apiClient.GetSupportedConversionsAsync();
        return supportedConversionsResponse.Content!.Adapt<SupportedConversions>();
    }

    public async Task ConvertFileToAllSupportedFormatsAsync(FileInfo file, SupportedConversions supportedConversions)
    {
        var sourceFormat = file.Extension.Replace(".", "");
        if (!supportedConversions.SourceFormatTo.ContainsKey(sourceFormat)) return;

        var supportedTargetFormats = supportedConversions.SourceFormatTo[sourceFormat];

        foreach (var targetFormat in supportedTargetFormats)
        {
            try
            {
                await ConvertFileToTargetFormatAsync(file, targetFormat);
            }
            catch (UnsuccessfulStatusCodeException ex)
            {
                _logger.LogError(ex, 
                    "{statusCode} '{reasonPhrase}' error from API while attempting to convert {fileName} to {targetFormat}. Skipping file...",
                    (int)ex.HttpResponseMessage.StatusCode, ex.HttpResponseMessage.ReasonPhrase, file.Name,
                    targetFormat);
            }
        }
    }

    public async Task ConvertFileToTargetFormatAsync(FileInfo file, string targetFormat)
    {
        if (file.Length == 0) return;

        _logger.LogInformation("Converting {fileName} to {targetFormat}", file.Name, targetFormat);

        var conversionResponse = await _apiClient.ConvertAsync(targetFormat,
            new StreamPart(file.OpenRead(), file.Name, MimeTypeMap.GetMimeType(file.Extension)));

        var conversion = conversionResponse.Content!;

        var downloadConvertedFileAsyncResponse = await _apiClient.DownloadConvertedFileAsync(conversion.Id);

        await using var convertedStream = await downloadConvertedFileAsyncResponse.Content!.ReadAsStreamAsync();

        await SaveFileAsync(
            Path.Combine(file.DirectoryName!,
                GetConvertedFileName(conversion.SourceFileName, conversion.TargetFormat,
                    conversion.ConvertedFileExtension)), convertedStream);

        _logger.LogInformation("{fileName} successfully converted to {targetFormat}!", file.Name, targetFormat);
    }

    private static async Task SaveFileAsync(string path, Stream convertedStream)
    {
        await using var fileStream = new FileStream(path, FileMode.Create);
        await convertedStream.CopyToAsync(fileStream);
        await convertedStream.DisposeAsync();
        await fileStream.DisposeAsync();
    }

    private static string GetConvertedFileName(string sourceFileName, string targetFormat,
        string convertedFileExtension)
    {
        return targetFormat == convertedFileExtension
            ? $"{sourceFileName}.ConvertX.To.{targetFormat}"
            : $"{sourceFileName}.ConvertX.To.{targetFormat}.{convertedFileExtension}";
    }
}