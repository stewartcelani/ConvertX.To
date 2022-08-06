using ConvertX.To.ConsoleUI.API.Services;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.ConsoleUI.API;

public class App
{
    private readonly IConversionService _conversionService;
    private readonly ILogger<App> _logger;

    private readonly DirectoryInfo _directory = new(@"C:\dev\convertx.to\sample_files\");

    public App(ILogger<App> logger, IConversionService conversionService)
    {
        _logger = logger;
        _conversionService = conversionService;
    }

    public async Task RunAsync(string[] args, CancellationToken ct = default)
    {
        var supportedConversions = await _conversionService.GetSupportedConversionsAsync();

        var files = _directory
            .GetFiles("*.*", SearchOption.AllDirectories)
            .Where(x => !x.Name.Contains("ConvertX.To"))
            .ToList();

        var parallelOptions = new ParallelOptions
        {
            CancellationToken = ct,
            MaxDegreeOfParallelism = 8
        };

        // TODO: Look into Refit CancellationToken's and how to pass them through all the way
        await Parallel.ForEachAsync(files, parallelOptions,
            async (file, ctx) =>
            {
                await _conversionService.ConvertFileToAllSupportedFormatsAsync(file, supportedConversions);
            });
    }
}