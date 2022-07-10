namespace ConvertX.To.API.Entities;

public class ConversionTask
{
    public Guid Id { get; init; }
    public string SourceFilePath { get; init; }
    public string SourceFileName { get; init; }
    public string FileNameWithoutExtension { get; init; }
    public string DirectoryName { get; init; }
    public string SourceFormat { get; init; }
    public string TargetFormat { get; init; }
    public DateTimeOffset RequestDate { get; init; } = DateTimeOffset.Now;
}