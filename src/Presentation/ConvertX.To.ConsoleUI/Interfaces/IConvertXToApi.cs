using ConvertX.To.API.Contracts.V1;
using Refit;
using ConvertX.To.API.Contracts.V1.Responses;

namespace ConvertX.To.ConsoleUI.Interfaces;

public interface IConvertXToApi
{
    [Get(ApiRoutesV1.Convert.Get)]
    Task<ApiResponse<SupportedConversionsResponse>> GetSupportedConversionsAsync();

    [Multipart]
    [Post(ApiRoutesV1.Convert.Post)]
    Task<ApiResponse<ConversionResponse>> ConvertAsync(string targetFormat, [AliasAs("file")] StreamPart stream);

    [Get(ApiRoutesV1.Files.Get)]
    Task<ApiResponse<HttpContent>> DownloadConvertedFileAsync(string conversionId);
}