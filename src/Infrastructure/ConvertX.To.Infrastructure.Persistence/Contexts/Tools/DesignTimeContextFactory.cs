using ConvertX.To.Application.Domain.Settings;
using ConvertX.To.Application.Helpers;
using ConvertX.To.Application.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.Infrastructure.Persistence.Contexts.Tools;

/// <summary>
/// Used by dotnet ef migrations tool
/// </summary>
public class DesignTimeContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        ILogger<ApplicationDbContext> logger = new Logger<ApplicationDbContext>(new LoggerFactory());
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(@"C:\dev\convertx.to\src\Presentation\ConvertX.To.API")
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        var databaseSettings = SettingsBinder.BindAndValidate<DatabaseSettings, DatabaseSettingsValidator>(configuration);
        
        builder.UseNpgsql(databaseSettings.ConnectionString);

        return new ApplicationDbContext(builder.Options, databaseSettings, logger);
    }
}