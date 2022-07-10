using System.Reflection;
using ConvertX.To.API.Data;
using ConvertX.To.API.Middleware;
using ConvertX.To.API.Settings;
using ConvertX.To.API.Validators;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ConvertX.To.API;

public static class StartupExtensions
{
    public static async Task RunPendingMigrationsAsync(this WebApplication app)
    {
        using var serviceScope = app.Services.CreateScope();
        var dataContext = serviceScope.ServiceProvider.GetRequiredService<DataContext>();

        if (dataContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory") // IntegrationTests
            if ((await dataContext.Database.GetPendingMigrationsAsync()).Any())
                await dataContext.Database.MigrateAsync();
    }
    
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
    
    public static void AddAzureSettings(this WebApplicationBuilder builder)
    {
        var azureSettings = new AzureSettings();
        builder.Configuration.AddJsonFile("appsettings.secret.json"); // Ignored by .gitignore
        builder.Configuration.GetSection(nameof(AzureSettings)).Bind(azureSettings);
        var azureSettingsValidator = new AzureSettingsValidator();
        var validationResult = azureSettingsValidator.Validate(azureSettings);
        if (!validationResult.IsValid) throw new ArgumentException($"Failed binding {nameof(AzureSettings)} using ConfigurationManager.GetSection", validationResult.Errors.First().PropertyName);
        builder.Services.AddSingleton(azureSettings);
    }

    public static void AddLocalFileServiceSettings(this WebApplicationBuilder builder)
    {
        var localFileServiceSettings = new LocalFileServiceSettings();
        builder.Configuration.GetSection(nameof(LocalFileServiceSettings)).Bind(localFileServiceSettings);
        if (string.IsNullOrEmpty(localFileServiceSettings.RootDirectory))
            throw new ArgumentException(
                $"Failed binding {nameof(LocalFileServiceSettings)} using ConfigurationManager.GetSection");
        builder.Services.AddSingleton(localFileServiceSettings);
    }

    public static void AddDatabaseSettings(this WebApplicationBuilder builder)
    {
        var databaseSettings = new DatabaseSettings();
        builder.Configuration.GetSection(nameof(DatabaseSettings)).Bind(databaseSettings);
        if (string.IsNullOrEmpty(databaseSettings.ConnectionString))
            throw new ArgumentException(
                $"Failed binding {nameof(DatabaseSettings)} using ConfigurationManager.GetSection");
        builder.Services.AddSingleton(databaseSettings);
    }
    
    public static void RegisterFiltersFromAssembly(this FilterCollection filterCollection, Assembly assembly)
    {
        var filters = assembly.ExportedTypes
            .Where(x => typeof(IFilterMetadata).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .ToList();
        filters.ForEach(filter => filterCollection.Add(filter));
    }
    
    public static void UseCustomExceptionHandlingMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}