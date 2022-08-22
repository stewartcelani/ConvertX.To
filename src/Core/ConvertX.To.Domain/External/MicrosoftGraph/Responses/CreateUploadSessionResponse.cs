using System.Text.Json.Serialization;

namespace ConvertX.To.Domain.External.MicrosoftGraph.Responses;

public class CreateUploadSessionResponse
{
    [JsonPropertyName("uploadUrl")]
    public string UploadUrl { get; set; }
}