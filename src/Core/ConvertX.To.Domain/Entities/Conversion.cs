using ConvertX.To.Domain.Common;

namespace ConvertX.To.Domain.Entities;

public class Conversion : AuditableBaseEntity<Guid>, ISoftDelete
{
    public string FileNameWithoutExtension { get; set; }
    public string SourceFormat { get; set; }
    public string ConvertedFormat { get; set; }
    public string SourceFileName => $"{FileNameWithoutExtension}.{SourceFormat}";
    public string ConvertedFileName => $"{FileNameWithoutExtension}.{ConvertedFormat}";
    public DateTimeOffset RequestDate { get; set; }
    public DateTimeOffset RequestCompleteDate { get; set; }
    
    public decimal RequestProcessingTime => (decimal)(RequestCompleteDate - RequestDate).TotalSeconds;
    public string UserIpAddress { get; set; }
    public int Downloads { get; set; } = 0;

    public DateTimeOffset? DateDeleted { get; set; }
}