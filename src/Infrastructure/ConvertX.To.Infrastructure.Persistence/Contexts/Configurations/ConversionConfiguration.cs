using ConvertX.To.Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConvertX.To.Infrastructure.Persistence.Contexts.Configurations;

public class ConversionConfiguration : IEntityTypeConfiguration<ConversionEntity>
{
    public void Configure(EntityTypeBuilder<ConversionEntity> builder)
    {
        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.SourceFormat)
            .IsRequired();

        builder.Property(x => x.ConvertedFormat)
            .IsRequired();

        builder.Property(x => x.SourceMegabytes)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.ConvertedMegabytes)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.DateRequestReceived)
            .IsRequired();

        builder.Property(x => x.DateRequestCompleted)
            .IsRequired();

        builder.Property(x => x.RequestSeconds)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Downloads)
            .HasDefaultValue(0);

        builder.Property(x => x.DateDeleted)
            .HasDefaultValue(null);

        builder.Property(x => x.DateUpdated)
            .HasDefaultValue(null);

        builder.Property(x => x.DateCreated)
            .HasDefaultValueSql("now()");
    }
}