namespace ConvertX.To.API.Contracts.V1.Responses;

public class ConversionResponse
{
    public string Id { get; set; }
    public string SourceFormat { get; set; }
    public string TargetFormat { get; set; }
    public string ConvertedFormat { get; set; }
    public decimal SourceMegabytes { get; set; }
    public decimal ConvertedMegabytes { get; set; }
    public DateTimeOffset RequestDate { get; set; }
    public DateTimeOffset RequestCompleteDate { get; set; }
    public decimal RequestSeconds { get; set; }
}