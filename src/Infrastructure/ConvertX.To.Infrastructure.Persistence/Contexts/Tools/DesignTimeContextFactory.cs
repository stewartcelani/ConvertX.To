using ConvertX.To.Domain.Settings;
using Microsoft.EntityFrameworkCore.Design;
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

        var databaseSettings = new DatabaseSettings
        {
            ConnectionString = "Server=mssql;Database=ConvertXTo;User=sa;Password=Password1!" 
        };

        return new ApplicationDbContext(databaseSettings, logger);
    }
}