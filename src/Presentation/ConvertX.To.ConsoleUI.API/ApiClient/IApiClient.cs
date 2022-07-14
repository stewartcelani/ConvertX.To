using ConvertX.To.API.Contracts.V1;
using ConvertX.To.API.Contracts.V1.Responses;
using Refit;

namespace ConvertX.To.ConsoleUI.API.ApiClient;

public interface IApiClient
{
    [Get(ApiRoutesV1.Convert.Get)]
    Task<ApiResponse<SupportedConversionsResponse>> GetSupportedConversionsAsync();

    [Multipart]
    [Post(ApiRoutesV1.Convert.Post)]
    Task<ApiResponse<ConversionResponse>> ConvertAsync(string targetFormat, [AliasAs("file")] StreamPart stream);

    [Get(ApiRoutesV1.Files.Get)]
    Task<ApiResponse<HttpContent>> DownloadConvertedFileAsync(string conversionId);
}