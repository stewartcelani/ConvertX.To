using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ConvertX.To.API.Logging;

public static class ConfigureLoggingExtension
{
    public static void ConfigureLogging(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices((ctx, serviceCollection) =>
        {

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(ctx.Configuration)
                .CreateLogger();

            serviceCollection.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(Log.Logger, true);
            });
        
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger<Program>>();
            serviceCollection.AddSingleton(typeof(ILogger), logger);
        });
    }
}