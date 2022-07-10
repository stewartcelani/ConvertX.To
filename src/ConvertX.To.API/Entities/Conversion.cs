using System.ComponentModel.DataAnnotations.Schema;

namespace ConvertX.To.API.Entities;

public class Conversion : IAuditable
{
    public Guid Id { get; set; }
    public string FileNameWithoutExtension { get; set; }
    public string SourceFormat { get; set; }
    public string TargetFormat { get; set; }
    public string SourceFileName { get; set; }
    public string ConvertedFileName { get; set; }
    public DateTimeOffset RequestDate { get; set; }
    public DateTimeOffset RequestCompleteDate { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal RequestProcessingTime => (decimal)(RequestCompleteDate - RequestDate).TotalSeconds;
    public string UserIpAddress { get; set; }
    public int Downloads { get; set; } = 0;
    
    public DateTimeOffset? DateDeleted { get; set; }
    public DateTimeOffset? DateUpdated { get; set; }
    public DateTimeOffset DateCreated { get; set; } = DateTimeOffset.Now;
}