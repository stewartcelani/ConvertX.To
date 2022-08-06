using System.Reflection;
using ConvertX.To.Application.Domain.Entities;
using ConvertX.To.Application.Domain.Entities.Common;
using ConvertX.To.Application.Domain.Settings;
using ConvertX.To.Application.Interfaces;
using ConvertX.To.Infrastructure.Persistence.Contexts.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.Infrastructure.Persistence.Contexts;

public class ApplicationDbContext : DbContext
{
    private readonly DatabaseSettings _databaseSettings;
    private readonly ILogger<ApplicationDbContext> _logger;

    public ApplicationDbContext(DatabaseSettings databaseSettings, ILogger<ApplicationDbContext> logger)
    {
        _databaseSettings = databaseSettings;
        _logger = logger;
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, DatabaseSettings databaseSettings,
        ILogger<ApplicationDbContext> logger)
        : base(options)
    {
        _databaseSettings = databaseSettings;
        _logger = logger;
    }

    public DbSet<ConversionEntity> Conversions { get; set; }
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<DateTimeOffset>()
            .HaveConversion<DateTimeOffsetConverter>();
    }    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.Now;

        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.DateCreated = now;
                    break;
                case EntityState.Modified:
                    entry.Entity.DateUpdated = now;
                    entry.Property(x => x.DateCreated).IsModified = false;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_databaseSettings.UseInMemoryDatabase)
        {
            optionsBuilder.UseInMemoryDatabase(nameof(ApplicationDbContext));
        }
        else
        {
            if (_databaseSettings.EnableSensitiveDataLogging)
            {
                optionsBuilder.UseNpgsql(_databaseSettings.ConnectionString,
                        sqlOptions => sqlOptions.EnableRetryOnFailure())
                    .EnableSensitiveDataLogging()
                    .LogTo(s => _logger.LogTrace(s));
            }
            else
            {
                optionsBuilder.UseNpgsql(_databaseSettings.ConnectionString,
                    sqlOptions => sqlOptions.EnableRetryOnFailure());
            }
        }
    }
}