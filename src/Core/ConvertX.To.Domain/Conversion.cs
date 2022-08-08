namespace ConvertX.To.Domain;

public class Conversion
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string SourceFormat { get; init; }
    public string TargetFormat { get; init; }
    public string ConvertedFormat { get; init; } // e.g. SourceFormat: pdf, TargetFormat: jpg, ConvertedFormat: zip
    public decimal SourceMegabytes { get; init; }
    public decimal ConvertedMegabytes { get; init; }
    public DateTimeOffset DateRequestReceived { get; init; }

    public DateTimeOffset DateRequestCompleted { get; init; }
    /*public ConversionOptions ConversionOptions { get; init; } = new ();*/
    
}