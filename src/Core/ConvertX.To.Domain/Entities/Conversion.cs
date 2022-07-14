using ConvertX.To.Domain.Common;

namespace ConvertX.To.Domain.Entities;

public sealed class Conversion : AuditableBaseEntity<Guid>, ISoftDelete
{
    public override Guid Id { get; set; } = Guid.NewGuid();
    public string FileNameWithoutExtension { get; set; }
    public string SourceFormat { get; set; }
    public string TargetFormat { get; set; }
    public string ConvertedFileExtension { get; set; } // e.g. SourceFormat: pdf, TargetFormat: jpg, ConvertedFileExtension: zip
    public DateTimeOffset RequestDate { get; set; }
    public DateTimeOffset RequestCompleteDate { get; set; }
    public int Downloads { get; set; } = 0;
    public DateTimeOffset? DateDeleted { get; set; }
    
    public string SourceFileName => $"{FileNameWithoutExtension}.{SourceFormat}";
    
    public string ConvertedFileName => TargetFormat == ConvertedFileExtension
        ? $"{FileNameWithoutExtension}.{TargetFormat}"
        : $"{FileNameWithoutExtension}.{TargetFormat}.{ConvertedFileExtension}";
    public decimal RequestProcessingTime => (decimal)(RequestCompleteDate - RequestDate).TotalSeconds;
}