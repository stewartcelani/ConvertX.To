using ConvertX.To.Application.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.ConsoleUI;

public class App : IHostedService
{
    private readonly ILogger<App> _logger;
    private readonly IConversionEngine _conversionEngine;
    private readonly IFileService _fileService;

    private DirectoryInfo _directory = new DirectoryInfo(@"C:\dev\convertx.to\sample_files");

    public App(ILogger<App> logger, IConversionEngine conversionEngine, IFileService fileService)
    {
        _logger = logger;
        _conversionEngine = conversionEngine;
        _fileService = fileService;
    }


    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("{Class}.{Method}",nameof(App), nameof(StartAsync));

        var targetFormat = "jpg";
        
        var supportedConversions = _conversionEngine.GetSupportedConversions();
        if (!supportedConversions.TargetFormatFrom.ContainsKey(targetFormat))
        {
            _logger.LogInformation("Converting to {targetFormat} is not supported", targetFormat);
            return;
        }
        
        var toTargetFormat = supportedConversions.TargetFormatFrom[targetFormat];

        var files = _directory.GetFiles().Where(x => !x.Name.Contains("ConvertX.To") && toTargetFormat.Contains(x.Extension.Replace(".",""))).ToList();

        var numberOfFilesConverted = 0;
        foreach (var fileInfo in files)
        {
            _logger.LogInformation("Converting {fileName} to {targetFormat}", fileInfo.Name, targetFormat);
            var convertedStream = await _conversionEngine.ConvertAsync(fileInfo.Extension.Replace(".", ""), targetFormat, fileInfo.OpenRead());
            await _fileService.SaveFileAsync(Path.Combine(fileInfo.DirectoryName!, $"{fileInfo.Name}.ConvertX.To.{targetFormat}"), convertedStream);
            numberOfFilesConverted++;
            _logger.LogInformation("{fileName} successfully converted to {targetFormat}!", fileInfo.Name, targetFormat);
        }
        
        _logger.LogInformation("Converted {numberOfFilesConverted} files to {targetFormat}.", numberOfFilesConverted, targetFormat);
        Console.ReadKey();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}