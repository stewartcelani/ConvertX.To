namespace ConvertX.To.API.Contracts.V1.Responses;

public class ConversionResponse
{
    public string Id { get; set; }
    public string FileNameWithoutExtension { get; set; }
    public string SourceFormat { get; set; }
    public string TargetFormat { get; set; }
    public string ConvertedFileExtension { get; set; } // e.g. SourceFormat: pdf, TargetFormat: jpg, ConvertedFileExtension: zip
    public DateTimeOffset RequestDate { get; set; }
    public DateTimeOffset RequestCompleteDate { get; set; }
    public string SourceFileName { get; set; } 
    public string ConvertedFileName { get; set; }
    public decimal RequestProcessingTime { get; set; }
}