using ConvertX.To.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConvertX.To.Infrastructure.Persistence.Contexts.Configurations;

public class ConversionConfiguration : IEntityTypeConfiguration<Conversion>
{
    public void Configure(EntityTypeBuilder<Conversion> builder)
    {
        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.FileNameWithoutExtension)
            .IsRequired();

        builder.Property(x => x.SourceFormat)
            .IsRequired();

        builder.Property(x => x.ConvertedFileExtension)
            .IsRequired();

        builder.Property(x => x.RequestDate)
            .IsRequired();

        builder.Property(x => x.RequestCompleteDate)
            .IsRequired();

        builder.Property(x => x.Downloads)
            .HasDefaultValue(0);

        builder.Property(x => x.DateDeleted)
            .HasDefaultValue(null);

        builder.Property(x => x.DateUpdated)
            .HasDefaultValue(null);

        builder.Property(x => x.DateCreated)
            .HasDefaultValueSql("getdate()");
    }
}