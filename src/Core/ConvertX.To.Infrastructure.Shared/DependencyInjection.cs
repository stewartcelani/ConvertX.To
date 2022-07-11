using ConvertX.To.Application.Converters;
using ConvertX.To.Application.Interfaces;
using ConvertX.To.Application.Settings;
using ConvertX.To.Application.Validators;
using ConvertX.To.Domain.Settings;
using ConvertX.To.Infrastructure.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        
        var fileServiceSettings = SettingsBinder.BindAndValidate<FileServiceSettings, FileServiceSettingsValidator>(configuration);
        services.AddSingleton(fileServiceSettings);

        var graphFileServiceSettings =
            SettingsBinder.BindAndValidate<GraphFileServiceSettings, GraphFileServiceSettingsValidator>(configuration);
        services.AddSingleton(graphFileServiceSettings);
        
        services.AddHttpClient();

        services.AddScoped<IFileService, LocalFileService>();
        services.AddScoped<IGraphFileService, SharePointService>();
        services.AddScoped<IConverterFactory, ConverterFactory>();
        services.AddScoped<IConversionEngine, ConversionEngine>();
        services.AddScoped<IConversionService, ConversionService>();
    }
    
}