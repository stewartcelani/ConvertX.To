using ConvertX.To.Application.Domain.Entities.Common;

namespace ConvertX.To.Application.Domain.Entities;

public sealed class ConversionEntity : AuditableBaseEntity<Guid>, ISoftDelete, IAggregateRoot
{
    public string SourceFormat { get; init; }
    public string TargetFormat { get; init; }
    public string ConvertedFormat { get; init; } // e.g. SourceFormat: pdf, TargetFormat: jpg, ConvertedFormat: zip
    public decimal SourceMegabytes { get; init; }
    public decimal ConvertedMegabytes { get; init; }
    public DateTimeOffset DateRequestReceived { get; init; }
    public DateTimeOffset DateRequestCompleted { get; init; }
    public decimal RequestSeconds { get; init; }
    public int Downloads { get; set; } = 0;
    public DateTimeOffset? DateDeleted { get; set; }
}