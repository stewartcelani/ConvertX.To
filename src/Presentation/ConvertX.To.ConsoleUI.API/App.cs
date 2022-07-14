using ConvertX.To.API.Contracts.V1.Responses;
using ConvertX.To.Application.Converters;
using ConvertX.To.Application.Interfaces;
using ConvertX.To.ConsoleUI.API.ApiClient;
using ConvertX.To.ConsoleUI.API.Exceptions;
using ConvertX.To.Domain.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MimeTypes.Core;
using Refit;

namespace ConvertX.To.ConsoleUI.API;

public class App : IHostedService
{
    private readonly ILogger<App> _logger;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IApiClient _apiClient;

    private DirectoryInfo _directory = new DirectoryInfo(@"C:\dev\convertx.to\sample_files");

    public App(ILogger<App> logger,
        IHostApplicationLifetime hostApplicationLifetime, IApiClient apiClient)
    {
        _logger = logger;
        _hostApplicationLifetime = hostApplicationLifetime;
        _apiClient = apiClient;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("{Class}.{Method}",nameof(App), nameof(StartAsync));

        var supportedConversionsResponse = await _apiClient.GetSupportedConversionsAsync();
        if (supportedConversionsResponse.Content is null)
            throw new ApiClientTechnicalException($"Null response content from {_apiClient.GetSupportedConversionsAsync()}",
                new NullReferenceException(nameof(SupportedConversions)));
        var supportedConversions = supportedConversionsResponse.Content;

        var files = _directory.GetFiles("*.*", SearchOption.AllDirectories).Where(x => !x.Name.Contains("ConvertX.To"))
            .ToList();

        foreach (var fileInfo in files)
        {
            await ConvertFileToAllSupportedFormatsAsync(fileInfo, supportedConversions);
        }

        _logger.LogInformation("End of program.");

        _hostApplicationLifetime.StopApplication();
    }

    private async Task ConvertFileToAllSupportedFormatsAsync(FileInfo fileInfo,
        SupportedConversionsResponse supportedConversions)
    {
        var sourceFormat = fileInfo.Extension.Replace(".", "");
        if (!supportedConversions.SourceFormatTo.ContainsKey(sourceFormat)) return;
        var supportedTargetFormats = supportedConversions.SourceFormatTo[sourceFormat];
        foreach (var targetFormat in supportedTargetFormats)
        {
            await ConvertFileToTargetFormat(fileInfo, targetFormat);
        }
    }

    private async Task ConvertFileToTargetFormat(FileInfo fileInfo, string targetFormat)
    {
        _logger.LogInformation("Converting {fileName} to {targetFormat}", fileInfo.Name, targetFormat);

        var conversionResponse = await _apiClient.ConvertAsync(targetFormat,
            new StreamPart(fileInfo.OpenRead(), fileInfo.Name, MimeTypeMap.GetMimeType(fileInfo.Extension)));

        var conversion = conversionResponse.Content!;

        var downloadConvertedFileAsyncResponse = await _apiClient.DownloadConvertedFileAsync(conversion.Id);
        if (downloadConvertedFileAsyncResponse.Content is null)
            throw new ApiClientTechnicalException(
                $"Null response content from {_apiClient.DownloadConvertedFileAsync(conversion.Id)}");

        await using var convertedStream = await downloadConvertedFileAsyncResponse.Content.ReadAsStreamAsync();

        var fileName = conversion.TargetFormat == conversion.ConvertedFileExtension
            ? $"{conversion.SourceFileName}.ConvertX.To.{conversion.TargetFormat}"
            : $"{conversion.SourceFileName}.ConvertX.To.{conversion.TargetFormat}.{conversion.ConvertedFileExtension}";

        await using var fileStream =
            new FileStream(Path.Combine(fileInfo.DirectoryName!, fileName), FileMode.Create);
        await convertedStream.CopyToAsync(fileStream);
        await convertedStream.DisposeAsync();
        await fileStream.DisposeAsync();

        _logger.LogInformation("{fileName} successfully converted to {targetFormat}!", fileInfo.Name, targetFormat);
    }


    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}