using ConvertX.To.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.ConsoleUI;

public class App
{
    private readonly ILogger<App> _logger;
    private readonly IConversionEngine _conversionEngine;

    private DirectoryInfo _directory = new DirectoryInfo(@"C:\dev\convertx.to\sample_files\pdf");

    public App(ILogger<App> logger, IConversionEngine conversionEngine)
    {
        _logger = logger;
        _conversionEngine = conversionEngine;
    }

    /// <summary>
    /// Just testing things from a Console app now to make sure I'm splitting the projects correctly
    /// Have an idea of adding a context menu based windows app that lets users right click a compatible file
    /// and convert to available formats
    /// </summary>
    public async Task RunAsync(string[] args)
    {
        _logger.LogInformation("{Class}.{Method}",nameof(App), nameof(RunAsync));
        
        var supportedConversions = _conversionEngine.GetSupportedConversions();
        var toPdf = supportedConversions.TargetFormatFrom["pdf"];

        var files = _directory.GetFiles().Where(x => toPdf.Contains(x.Extension.Replace(".",""))).ToList();
        if (files.Any()) EnsureDirectory(Path.Combine(_directory.FullName, "output"));
        
        foreach (var fileInfo in files)
        {
            var convertAsync = await _conversionEngine.ConvertAsync(fileInfo.Extension.Replace(".", ""), "pdf", fileInfo.OpenRead());
            
            _logger.LogDebug(fileInfo.Name);
        }

        Console.ReadKey();
    }

    private void EnsureDirectory(string path)
    {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    }
    

}