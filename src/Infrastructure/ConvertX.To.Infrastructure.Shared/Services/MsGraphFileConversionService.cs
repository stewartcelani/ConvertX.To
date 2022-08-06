using System.Net.Http.Headers;
using ConvertX.To.Application.Interfaces;
using Newtonsoft.Json;
using Microsoft.Graph;
using Azure.Identity;
using ConvertX.To.Application.Domain.Settings;
using ConvertX.To.Application.Exceptions;
using ConvertX.To.Infrastructure.Shared.Http;
using Microsoft.Extensions.Logging;
using MimeTypes.Core;
using Polly.Retry;

namespace ConvertX.To.Infrastructure.Shared.Services;

/// <summary>
/// Service using the Microsoft Graph (beta) Files -> Drive Items -> Convert file API
/// to request file contents in supported formats
/// https://docs.microsoft.com/en-us/graph/api/driveitem-get-content-format?view=graph-rest-beta&tabs=http
/// HTTP GET /drive/items/{item-id}/content?format={format}
/// </summary>
public class MsGraphFileConversionService : IMsGraphFileConversionService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly MsGraphSettings _msGraphSettings;
    private readonly ILogger<MsGraphFileConversionService> _logger;
    private readonly AsyncRetryPolicy _asyncRetryPolicy;

    private HttpClient? _httpClient;
    private GraphServiceClient? _graphServiceClient;

    public MsGraphFileConversionService(IHttpClientFactory httpClientFactory, MsGraphSettings msGraphSettings, ILogger<MsGraphFileConversionService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _msGraphSettings = msGraphSettings;
        _logger = logger;
        _asyncRetryPolicy = HttpClientRetryPolicy.GetPolicy(_logger, 3, 2);
    }
    
    public async Task<string> UploadFileAsync(string sourceFormat, Stream source)
    {
        if (source.Length > 3900000) return await UploadLargeFileAsync(sourceFormat, source); // Using Graph SDK for > 3.9 MB
        
        var httpClient = await CreateAuthorizedHttpClient();
        var tempFileName = $"{Guid.NewGuid().ToString().Replace("-", "")}.{sourceFormat}";
        var requestUrl = $"{_msGraphSettings.GraphEndpoint}/root:/{tempFileName}:/content";
        var requestContent = new StreamContent(source);
        requestContent.Headers.ContentType = new MediaTypeHeaderValue(MimeTypeMap.GetMimeType(sourceFormat));
        var response =
            await _asyncRetryPolicy.ExecuteAsync(async () => await httpClient.PutAsync(requestUrl, requestContent));
        if (!response.IsSuccessStatusCode)
            throw new MsGraphUploadFileException(response);
        var responseBody = await response.Content.ReadAsStringAsync();
        dynamic fileResponse = JsonConvert.DeserializeObject(responseBody);
        return fileResponse?.id ?? throw new NullReferenceException(nameof(fileResponse.id));
    }

    public async Task<Stream> GetFileInTargetFormatAsync(string fileId, string targetFormat)
    {
        var httpClient = await CreateAuthorizedHttpClient();
        
        var requestUrl = $"{_msGraphSettings.GraphEndpoint}/{fileId}/content?format={targetFormat}";
        
        if (targetFormat.Equals("jpg"))
            requestUrl += "&width=1920&height=1080";
        
        var response = await _asyncRetryPolicy.ExecuteAsync(async () => await httpClient.GetAsync(requestUrl));
        
        if (response.IsSuccessStatusCode) return await response.Content.ReadAsStreamAsync();
        
        try
        {
            await DeleteFileAsync(fileId);
        }
        catch (MsGraphDeleteFileException ex)
        {
            throw new MsGraphGetFileInTargetFormatException(response, ex);
        }
            
        throw new MsGraphGetFileInTargetFormatException(response);
    }

    public async Task DeleteFileAsync(string fileId)
    {
        var httpClient = await CreateAuthorizedHttpClient();
        var requestUrl = $"{_msGraphSettings.GraphEndpoint}/{fileId}";
        var response = await _asyncRetryPolicy.ExecuteAsync(async () => await httpClient.DeleteAsync(requestUrl));
        if (!response.IsSuccessStatusCode)
            throw new MsGraphDeleteFileException(response);
    }
    
    /// <summary>
    /// File sizes >= 4 MB require file sessions so using the Graph SDK for this
    /// </summary>
    private async Task<string> UploadLargeFileAsync(string sourceFormat, Stream stream)
    {
        var graphServiceClient = CreateGraphServiceClient();
        var tempFileName = $"{Guid.NewGuid().ToString().Replace("-", "")}.{sourceFormat}";
        var requestUrl = $"{_msGraphSettings.GraphEndpoint}/root:/{tempFileName}:/microsoft.graph.createUploadSession";
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, requestUrl);
        await graphServiceClient.AuthenticationProvider.AuthenticateRequestAsync(httpRequestMessage);
        var httpResponseMessage = await graphServiceClient.HttpProvider.SendAsync(httpRequestMessage);
        var content = await httpResponseMessage.Content.ReadAsStringAsync();
        var uploadSession = graphServiceClient.HttpProvider.Serializer.DeserializeObject<UploadSession>(content);    
        var largeFileUploadTask = new LargeFileUploadTask<DriveItem>(uploadSession, stream);
        try
        {
            var uploadResult = await largeFileUploadTask.UploadAsync();

            if (!uploadResult.UploadSucceeded) throw new MsGraphUploadLargeFileException(uploadResult);

            await stream.DisposeAsync();
            return uploadResult.ItemResponse.Id;
        }
        catch (ServiceException e)
        {
            _logger.LogError(e.Message, e);
            await stream.DisposeAsync();
            throw;
        }
    }
    
    private GraphServiceClient CreateGraphServiceClient()
    {
        if (_graphServiceClient is not null) return _graphServiceClient;

        var options = new TokenCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
        };
        var clientSecretCredential = new ClientSecretCredential(_msGraphSettings.TenantId, _msGraphSettings.ClientId, _msGraphSettings.ClientSecret, options);
        var scopes = new[]
        {
            _msGraphSettings.Scope
        };
        _graphServiceClient = new GraphServiceClient(clientSecretCredential, scopes);
        
        return _graphServiceClient;
    }
    
    private async Task<HttpClient> CreateAuthorizedHttpClient()
    {
        if (_httpClient is not null) return _httpClient;

        var token = await GetAccessTokenAsync();
        _httpClient = _httpClientFactory.CreateClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        return _httpClient;
    }
    
    private async Task<string> GetAccessTokenAsync()
    {
        var values = new List<KeyValuePair<string, string>>
        {
            new("client_id", _msGraphSettings.ClientId),
            new("client_secret", _msGraphSettings.ClientSecret),
            new("scope", _msGraphSettings.Scope),
            new("grant_type", "client_credentials"),
        };

        using var client = _httpClientFactory.CreateClient();

        var requestUrl = $"{_msGraphSettings.AuthenticationEndpoint}/{_msGraphSettings.TenantId}/oauth2/v2.0/token";
        var requestContent = new FormUrlEncodedContent(values);

        var response = await client.PostAsync(requestUrl, requestContent);
        if (!response.IsSuccessStatusCode)
            throw new MsGraphAuthorizationException(response);
        var responseBody = await response.Content.ReadAsStringAsync();

        dynamic tokenResponse = JsonConvert.DeserializeObject(responseBody);
        return tokenResponse?.access_token ?? throw new NullReferenceException(nameof(tokenResponse.access_token));
    }
    
}