using ConvertX.To.Application.Converters;
using ConvertX.To.Application.Domain.Settings;
using ConvertX.To.Application.Helpers;
using ConvertX.To.Application.Interfaces;
using ConvertX.To.Application.Logging;
using ConvertX.To.Application.Validators;
using ConvertX.To.Infrastructure.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Polly;
using Polly.Extensions.Http;
using Serilog;

namespace ConvertX.To.Infrastructure.Shared;

public static class DependencyInjection
{
    public static void AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(Log.Logger, true);
        });
        services.AddTransient(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));

        var msGraphSettings =
            SettingsBinder.BindAndValidate<MsGraphSettings, MsGraphSettingsValidator>(configuration);
        services.AddSingleton(msGraphSettings);

        var conversionStorageSettings = SettingsBinder.Bind<ConversionStorageSettings>(configuration);
        services.AddSingleton(conversionStorageSettings);

        services.AddHttpClient();

        ILoggerAdapter<MsGraphFileConversionService> msGraphLogger =
            new LoggerAdapter<MsGraphFileConversionService>(new LoggerFactory().CreateLogger<MsGraphFileConversionService>());
        
        var transientRetryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(5,
                retryAttempt => TimeSpan.FromSeconds(retryAttempt * 2),
                (exception, sleepDuration, retryCount, context) =>
                {
                    var message =
                        $"Expecting this to be a transient error. Retry {retryCount}/{5} in {sleepDuration.Seconds} seconds.";
                    msGraphLogger?.LogError(exception.Exception, message);
                });

        services.AddTransient<RetryHandler>();
        
        services.AddHttpClient(nameof(MsGraphFileConversionService))
            .AddHttpMessageHandler<RetryHandler>() // Was making my own when came across Microsoft's own 429 retry handler used by their SDK, might as well use that!
            .AddPolicyHandler(transientRetryPolicy);

        services.AddScoped<IFileService, LocalFileService>();
        services.AddScoped<IConversionStorageService, LocalConversionStorageService>();
        services.AddScoped<IMsGraphFileConversionService, MsGraphFileConversionService>();
        services.AddScoped<IConverterFactory, ConverterFactory>();
        services.AddScoped<IConversionEngine, ConversionEngine>();
        services.AddScoped<IConversionService, ConversionService>();
        services.AddScoped<IConversionLifecycleManagerService, ConversionLifecycleManagerService>();
    }
}