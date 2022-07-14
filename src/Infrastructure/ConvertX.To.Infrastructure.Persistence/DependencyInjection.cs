using ConvertX.To.Application.Helpers;
using ConvertX.To.Application.Validators;
using ConvertX.To.Domain.Settings;
using ConvertX.To.Infrastructure.Persistence.Contexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConvertX.To.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var ttlSettings =
            SettingsBinder.BindAndValidate<TimeToLiveSettings, TimeToLiveSettingsValidator>(configuration);
        services.AddSingleton(ttlSettings);
    
        var databaseSettings = SettingsBinder.BindAndValidate<DatabaseSettings, DatabaseSettingsValidator>(configuration); 
        services.AddSingleton(databaseSettings);

        services.AddDbContext<ApplicationDbContext>(); // Config is within class as DI is required
        
        /*#region Repositories
        services.AddTransient<IConversionRepository, ConversionRepositoryAsync>();
        #endregion*/
    }

}