using ConvertX.To.Domain.Common;

namespace ConvertX.To.Domain.Entities;

public sealed class Conversion : AuditableBaseEntity<Guid>, ISoftDelete, IAggregateRoot
{
    public override Guid Id { get; set; } = Guid.NewGuid();
    public string SourceFormat { get; init; }
    public string TargetFormat { get; init; }
    public string ConvertedFormat { get; init; } // e.g. SourceFormat: pdf, TargetFormat: jpg, ConvertedFormat: zip
    public decimal SourceMegabytes { get; init; }
    public decimal ConvertedMegabytes { get; init; }
    public DateTimeOffset RequestDate { get; init; }
    public DateTimeOffset RequestCompleteDate { get; init; }
    public decimal RequestSeconds { get; init; }
    public int Downloads { get; set; } = 0;
    public DateTimeOffset? DateDeleted { get; set; }
}