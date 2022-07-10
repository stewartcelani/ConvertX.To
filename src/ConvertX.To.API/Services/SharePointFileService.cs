using System.Net.Http.Headers;
using ConvertX.To.API.Exceptions.Technical;
using ConvertX.To.API.Settings;
using MimeTypes.Core;
using Newtonsoft.Json;

namespace ConvertX.To.API.Services;

public class SharePointFileService : IAzureFileService
{
    private readonly ILogger<SharePointFileService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AzureSettings _azureSettings;

    private HttpClient? _httpClient;

    public SharePointFileService(ILogger<SharePointFileService> logger, IHttpClientFactory httpClientFactory, AzureSettings azureSettings)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _azureSettings = azureSettings;
    }

    public async Task<string> UploadFileAsync(string filePath)
    {
        var httpClient = await CreateAuthorizedHttpClient();
        var tempFileName = $"{Guid.NewGuid().ToString().Replace("-", "")}.{Path.GetExtension(filePath)}";
        var requestUrl = $"{_azureSettings.GraphEndpoint}/root:/{tempFileName}:/content";
        await using var stream = File.Open(filePath, FileMode.Open);
        var requestContent = new StreamContent(stream);
        requestContent.Headers.ContentType = new MediaTypeHeaderValue(MimeTypeMap.GetMimeType(Path.GetExtension(filePath)));
        var response = await httpClient.PutAsync(requestUrl, requestContent);
        var responseBody = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new AzureUploadFileException(
                $"Upload file failed with status {response.StatusCode} and message {responseBody}");
        dynamic fileResponse = JsonConvert.DeserializeObject(responseBody);
        return fileResponse?.id ?? throw new AzureUploadFileException($"Error deserializing response sourceFormat {requestUrl}");
    }

    public async Task<Stream> GetConvertedFileAsync(string fileId, string targetFormat)
    {
        var httpClient = await CreateAuthorizedHttpClient();
        var requestUrl = $"{_azureSettings.GraphEndpoint}/{fileId}/content?format={targetFormat}";
        var response = await httpClient.GetAsync(requestUrl);
        if (!response.IsSuccessStatusCode)
            throw new AzureDownloadConvertedFileException(
                $"Downloading converted file failed with status {response.StatusCode} and message {await response.Content.ReadAsStringAsync()}");
        return await response.Content.ReadAsStreamAsync();
    }

    public async Task DeleteFileAsync(string fileId)
    {
        var httpClient = await CreateAuthorizedHttpClient();
        var requestUrl = $"{_azureSettings.GraphEndpoint}/{fileId}";
        var response = await httpClient.DeleteAsync(requestUrl);
        if (!response.IsSuccessStatusCode)
            throw new AzureDeleteFileException(
                $"Deleting file with id {fileId} failed with status {response.StatusCode} and message {response.Content.ReadAsStringAsync()}");
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
            new("client_id", _azureSettings.ClientId),
            new("client_secret", _azureSettings.ClientSecret),
            new("scope", _azureSettings.Scope),
            new("grant_type", "client_credentials"),
        };

        using var client = _httpClientFactory.CreateClient();

        var requestUrl = $"{_azureSettings.AuthenticationEndpoint}/{_azureSettings.TenantId}/oauth2/v2.0/token";
        var requestContent = new FormUrlEncodedContent(values);

        var response = await client.PostAsync(requestUrl, requestContent);
        var responseBody = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new AzureAuthorizationException($"Non-200 response from {requestUrl}: {responseBody}");

        dynamic tokenResponse = JsonConvert.DeserializeObject(responseBody);
        if (tokenResponse is null || tokenResponse?.access_token is null)
            throw new AzureAuthorizationException($"Error deserializing response from {requestUrl}");
        return tokenResponse?.access_token;
    }
    
}