using ConvertX.To.API.Entities;
using ConvertX.To.API.Settings;
using Microsoft.EntityFrameworkCore;

namespace ConvertX.To.API.Data;

public class DataContext : DbContext
{
    private readonly DatabaseSettings _databaseSettings;
    private readonly ILogger<DataContext> _logger;

    public DataContext(DatabaseSettings databaseSettings, ILogger<DataContext> logger)
    {
        _databaseSettings = databaseSettings;
        _logger = logger;
    }

    public DataContext(DbContextOptions<DataContext> options, DatabaseSettings databaseSettings,
        ILogger<DataContext> logger)
        : base(options)
    {
        _databaseSettings = databaseSettings;
        _logger = logger;
    }

    public DbSet<Conversion> Conversions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_databaseSettings.ConnectionString, sqlOptions => sqlOptions.EnableRetryOnFailure())
            .EnableSensitiveDataLogging()
            .LogTo(x => _logger.LogTrace(x));
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.Now;

        foreach (var auditable in ChangeTracker.Entries()
                     .Where(x => x.State == EntityState.Added)
                     .Select(x => x.Entity).Where(x => x is IAuditable).Cast<IAuditable>())
            auditable.DateCreated = now;

        foreach (var auditable in ChangeTracker.Entries()
                     .Where(x => x.State == EntityState.Modified)
                     .Select(x => x.Entity).Where(x => x is IAuditable).Cast<IAuditable>())
        {
            auditable.DateUpdated = now;
            Entry(auditable).Property(x => x.DateCreated).IsModified = false;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}