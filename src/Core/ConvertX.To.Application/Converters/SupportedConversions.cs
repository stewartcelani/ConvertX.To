namespace ConvertX.To.Application.Converters;

/// <summary>
/// Dynamic mapping of all supported conversions using reflection
/// See SupportedConversions.json
/// </summary>
public class SupportedConversions
{
    public object TargetFormatFrom { get; set; }
    public object SourceFormatTo { get; set; }
}