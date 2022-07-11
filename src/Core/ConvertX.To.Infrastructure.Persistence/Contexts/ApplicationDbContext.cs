using System.Reflection;
using ConvertX.To.Domain.Common;
using ConvertX.To.Domain.Entities;
using ConvertX.To.Domain.Settings;
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

    public DbSet<Conversion> Conversions { get; set; }
    
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
                    //Entry(entry.Entity).Property(x => x.DateUpdated).IsModified = false;
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
                optionsBuilder.UseSqlServer(_databaseSettings.ConnectionString,
                    sqlOptions => sqlOptions.EnableRetryOnFailure());
            }
            else
            {
                optionsBuilder.UseSqlServer(_databaseSettings.ConnectionString,
                        sqlOptions => sqlOptions.EnableRetryOnFailure())
                    .EnableSensitiveDataLogging()
                    .LogTo(s => _logger.LogTrace(s));
            }
        }
    }
}