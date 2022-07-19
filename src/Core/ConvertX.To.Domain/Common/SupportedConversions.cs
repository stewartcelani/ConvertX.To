namespace ConvertX.To.Domain.Common;

/// <summary>
/// Dynamic mapping of all supported conversions using reflection
/// See SupportedConversions.json for example
/// </summary>
public class SupportedConversions
{
    public Dictionary<string, List<string>> TargetFormatFrom { get; set; }
    public Dictionary<string, List<string>> SourceFormatTo { get; set; }
}