namespace ConvertX.To.Application.Converters;

/// <summary>
/// Dynamic mapping of all supported conversions using reflection
/// See SupportedConversions.json
/// </summary>
public class SupportedConversions
{
    public Dictionary<string, List<string>> TargetFormatFrom { get; set; }
    public Dictionary<string, List<string>> SourceFormatTo { get; set; }
}