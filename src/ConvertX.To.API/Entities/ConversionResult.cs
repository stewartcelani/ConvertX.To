namespace ConvertX.To.API.Entities;

public class ConversionResult : ConversionTask
{
    public string ConvertedFilePath { get; set; }
    public string ConvertedFileName { get; set; }
    public DateTimeOffset RequestCompleteDate { get; set; } = DateTimeOffset.Now;
}