using ConvertX.To.API.Contracts.V1.Responses;
using ConvertX.To.ConsoleUI.API.Exceptions;
using Polly.Timeout;
using Refit;

namespace ConvertX.To.ConsoleUI.API.ApiClient;

public class ApiClient : IApiClient
{
    private readonly IApiClient _apiClient;

    public ApiClient(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<ApiResponse<SupportedConversionsResponse>> GetSupportedConversionsAsync()
    {
        try
        {
            return await _apiClient.GetSupportedConversionsAsync();
        }
        catch (ValidationApiException ex)
        {
            throw new ApiClientBusinessException(ex);
        }
        catch (ApiException ex)
        {
            throw new ApiClientTechnicalException(ex);
        }
        catch (Exception ex) when (ex is TimeoutRejectedException)
        {
            throw new ApiClientTechnicalException(ex.Message, ex);
        }
    }

    public async Task<ApiResponse<ConversionResponse>> ConvertAsync(string targetFormat, StreamPart stream)
    {
        try
        {
            return await _apiClient.ConvertAsync(targetFormat, stream);
        }
        catch (ValidationApiException ex)
        {
            throw new ApiClientBusinessException(ex);
        }
        catch (ApiException ex)
        {
            throw new ApiClientTechnicalException(ex);
        }
        catch (Exception ex) when (ex is TimeoutRejectedException)
        {
            throw new ApiClientTechnicalException(ex.Message, ex);
        }
    }

    public async Task<ApiResponse<HttpContent>> DownloadConvertedFileAsync(string conversionId)
    {
        try
        {
            return await _apiClient.DownloadConvertedFileAsync(conversionId);
        }
        catch (ValidationApiException ex)
        {
            throw new ApiClientBusinessException(ex);
        }
        catch (ApiException ex)
        {
            throw new ApiClientTechnicalException(ex);
        }
        catch (Exception ex) when (ex is TimeoutRejectedException)
        {
            throw new ApiClientTechnicalException(ex.Message, ex);
        }
    }
}