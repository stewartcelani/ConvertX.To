using ConvertX.To.Application.Helpers;
using ConvertX.To.Application.Validators;
using ConvertX.To.Domain.Settings;
using ConvertX.To.Infrastructure.Persistence.Contexts;
using ConvertX.To.Infrastructure.Persistence.Cron;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConvertX.To.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var ttlSettings =
            SettingsBinder.BindAndValidate<ConversionLifecycleManagerSettings, ConversionLifecycleManagerSettingsValidator>(configuration);
        services.AddSingleton(ttlSettings);

        var databaseSettings =
            SettingsBinder.BindAndValidate<DatabaseSettings, DatabaseSettingsValidator>(configuration);
        services.AddSingleton(databaseSettings);

        services.AddScoped<ApplicationDbContext>(); // Config is within class as DI is required
        
        services.AddHangfire(x => x.UseSqlServerStorage(databaseSettings.ConnectionString,
            new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true,
            }));
        services.AddHangfireServer();

        services.AddScoped<ConversionLifecycleManagerScheduledTask>();

        /*#region Repositories
        services.AddTransient<IConversionRepository, ConversionRepositoryAsync>();
        #endregion*/
    }
}