using ConvertX.To.Domain.Common;

namespace ConvertX.To.Domain.Entities;

public sealed class Conversion : AuditableBaseEntity<Guid>, ISoftDelete
{
    #region Database Fields
    public override Guid Id { get; set; } = Guid.NewGuid();
    public string FileNameWithoutExtension { get; init; }
    public string SourceFormat { get; init; }
    public string TargetFormat { get; init; }
    public string ConvertedFileExtension { get; init; } // e.g. SourceFormat: pdf, TargetFormat: jpg, ConvertedFileExtension: zip
    public DateTimeOffset RequestDate { get; init; }
    public DateTimeOffset RequestCompleteDate { get; init; }
    public int Downloads { get; set; } = 0;
    public DateTimeOffset? DateDeleted { get; set; }
    #endregion
    
    #region C# Computed
    public string SourceFileName => $"{FileNameWithoutExtension}.{SourceFormat}";
    public string ConvertedFileName => TargetFormat == ConvertedFileExtension
        ? $"{FileNameWithoutExtension}.{TargetFormat}"
        : $"{FileNameWithoutExtension}.{TargetFormat}.{ConvertedFileExtension}";
    public decimal RequestProcessingTime => (decimal)(RequestCompleteDate - RequestDate).TotalSeconds;
    #endregion
    
    
}