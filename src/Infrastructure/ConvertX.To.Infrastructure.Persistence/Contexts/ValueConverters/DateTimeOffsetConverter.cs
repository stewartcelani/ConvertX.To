using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ConvertX.To.Infrastructure.Persistence.Contexts.ValueConverters;

/// <summary>
/// When converting from Mssql to Postgres I encountered the following error when trying to save DateTimeOffsets:
/// 'Cannot write DateTimeOffset with Offset=09:30:00 to PostgreSQL type 'timestamp with time zone', only offset 0 (UTC) is supported. Note that it's not possible to mix DateTimes with different Kinds in an array/range. See the Npgsql.EnableLegacyTimestampBehavior AppContext switch to enable legacy behavior.'
/// This is a great workaround from the following issue:
/// https://github.com/npgsql/npgsql/issues/4176
/// Which makes use of inbuilt EF ValueConverters:
/// https://docs.microsoft.com/en-us/ef/core/modeling/value-conversions?tabs=data-annotations#bulk-configuring-a-value-converter
/// </summary>
public class DateTimeOffsetConverter : ValueConverter<DateTimeOffset, DateTimeOffset>
{
    public DateTimeOffsetConverter()
        : base(
            d => d.ToUniversalTime(),
            d => d.ToUniversalTime())
    {
    }
}