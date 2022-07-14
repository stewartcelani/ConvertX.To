namespace ConvertX.To.API.Contracts.V1.Responses;

public class SupportedConversionsResponse
{
    public Dictionary<string, List<string>> TargetFormatFrom { get; set; }
    public Dictionary<string, List<string>> SourceFormatTo { get; set; }
}