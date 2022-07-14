using ConvertX.To.Application.Converters;
using ConvertX.To.Application.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.ConsoleUI;

public class App : IHostedService
{
    private readonly ILogger<App> _logger;
    private readonly IConversionEngine _conversionEngine;
    private readonly IFileService _fileService;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    private DirectoryInfo _directory = new DirectoryInfo(@"C:\dev\convertx.to\sample_files");

    public App(ILogger<App> logger, IConversionEngine conversionEngine, IFileService fileService, IHostApplicationLifetime hostApplicationLifetime)
    {
        _logger = logger;
        _conversionEngine = conversionEngine;
        _fileService = fileService;
        _hostApplicationLifetime = hostApplicationLifetime;
    }


    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("{Class}.{Method}",nameof(App), nameof(StartAsync));
        
        var supportedConversions = _conversionEngine.GetSupportedConversions();
        
        var files = _directory.GetFiles("*.*", SearchOption.AllDirectories).Where(x => !x.Name.Contains("ConvertX.To")).ToList();

        foreach (var fileInfo in files)
        {
            await ConvertFileToAllSupportedFormatsAsync(fileInfo, supportedConversions);
        }
        
        _hostApplicationLifetime.StopApplication();
    }

    private async Task ConvertFileToAllSupportedFormatsAsync(FileInfo fileInfo, SupportedConversions supportedConversions)
    {
        var sourceFormat = fileInfo.Extension.Replace(".", "");
        if (!supportedConversions.SourceFormatTo.ContainsKey(sourceFormat)) return;
        var supportedTargetFormats = supportedConversions.SourceFormatTo[sourceFormat];
        foreach (var targetFormat in supportedTargetFormats)
        {
            _logger.LogInformation("Converting {fileName} to {targetFormat}", fileInfo.Name, targetFormat);
            var convertedStream = await _conversionEngine.ConvertAsync(sourceFormat, targetFormat, fileInfo.OpenRead());
            await _fileService.SaveFileAsync(Path.Combine(fileInfo.DirectoryName!, $"{fileInfo.Name}.ConvertX.To.{targetFormat}"), convertedStream);
            _logger.LogInformation("{fileName} successfully converted to {targetFormat}!", fileInfo.Name, targetFormat);
        }
    }
    
    

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}