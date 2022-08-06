using ConvertX.To.Application.Domain.Settings;
using ConvertX.To.Application.Helpers;
using ConvertX.To.Application.Interfaces.Repositories;
using ConvertX.To.Application.Validators;
using ConvertX.To.Infrastructure.Persistence.Contexts;
using ConvertX.To.Infrastructure.Persistence.Cron;
using ConvertX.To.Infrastructure.Persistence.Repositories;
using Hangfire;
using Hangfire.PostgreSql;
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
        
        
        
        services.AddHangfire(x => x.UsePostgreSqlStorage(databaseSettings.ConnectionString));
        services.AddHangfireServer();

        services.AddScoped<ConversionLifecycleManagerServiceScheduledTask>();

        #region Repositories
        services.AddTransient<IConversionRepository, ConversionRepository>();
        #endregion
    }
}