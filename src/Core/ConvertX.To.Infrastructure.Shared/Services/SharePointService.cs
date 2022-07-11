using System.Net.Http.Headers;
using ConvertX.To.Application.Exceptions.Technical;
using ConvertX.To.Application.Interfaces;
using ConvertX.To.Domain.Settings;
using MimeTypes.Core;
using Newtonsoft.Json;

namespace ConvertX.To.Infrastructure.Shared.Services;

public class SharePointService : IGraphFileService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GraphFileServiceSettings _graphFileServiceSettings;

    private HttpClient? _httpClient;

    public SharePointService(IHttpClientFactory httpClientFactory, GraphFileServiceSettings graphFileServiceSettings)
    {
        _httpClientFactory = httpClientFactory;
        _graphFileServiceSettings = graphFileServiceSettings;
    }

    public async Task<string> UploadFileAsync(string filePath)
    {
        var httpClient = await CreateAuthorizedHttpClient();
        var tempFileName = $"{Guid.NewGuid().ToString().Replace("-", "")}.{Path.GetExtension(filePath)}";
        var requestUrl = $"{_graphFileServiceSettings.GraphEndpoint}/root:/{tempFileName}:/content";
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
        var requestUrl = $"{_graphFileServiceSettings.GraphEndpoint}/{fileId}/content?format={targetFormat}";
        var response = await httpClient.GetAsync(requestUrl);
        if (!response.IsSuccessStatusCode)
            throw new AzureDownloadConvertedFileException(
                $"Downloading converted file failed with status {response.StatusCode} and message {await response.Content.ReadAsStringAsync()}");
        return await response.Content.ReadAsStreamAsync();
    }

    public async Task DeleteFileAsync(string fileId)
    {
        var httpClient = await CreateAuthorizedHttpClient();
        var requestUrl = $"{_graphFileServiceSettings.GraphEndpoint}/{fileId}";
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
            new("client_id", _graphFileServiceSettings.ClientId),
            new("client_secret", _graphFileServiceSettings.ClientSecret),
            new("scope", _graphFileServiceSettings.Scope),
            new("grant_type", "client_credentials"),
        };

        using var client = _httpClientFactory.CreateClient();

        var requestUrl = $"{_graphFileServiceSettings.AuthenticationEndpoint}/{_graphFileServiceSettings.TenantId}/oauth2/v2.0/token";
        var requestContent = new FormUrlEncodedContent(values);

        var response = await client.PostAsync(requestUrl, requestContent);
        var responseBody = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new AzureAuthorizationException($"Non-200 response from {requestUrl}: {responseBody}");

        dynamic tokenResponse = JsonConvert.DeserializeObject(responseBody);
        return tokenResponse?.access_token ?? throw new AzureAuthorizationException($"Error deserializing response from {requestUrl}");
    }
    
}