using System.Net.Http.Headers;
using ConvertX.To.API.Exceptions;
using ConvertX.To.API.Settings;
using MimeTypes.Core;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace ConvertX.To.API.Converters;

public abstract class AzureConverter : IConverter
{
    private readonly string _sourceFormat;
    private readonly string _targetFormat;
    private readonly AzureSettings _azureSettings;
    protected readonly ILogger _logger;
    
    private HttpClient? _httpClient;

    protected AzureConverter(string sourceFormat, string targetFormat, AzureSettings azureSettings, ILogger logger)
    {
        _sourceFormat = sourceFormat;
        _targetFormat = targetFormat;
        _logger = logger;
        _azureSettings = azureSettings;
    }

    public virtual async Task<FileInfo> ConvertAsync(FileInfo file)
    {
        _logger.LogDebug($"{nameof(AzureConverter)}.{nameof(ConvertAsync)}");
        var fileId = await UploadFileAsync(file);
        var directoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var fileName = $"{Path.GetFileNameWithoutExtension(file.FullName)}.{_targetFormat}";
        var convertedFile = await DownloadConvertedFileAsync(directoryPath, fileName, fileId, _targetFormat);
        // TODO: Delete file from sharepoint now it is no longer needed.
        return convertedFile;
    }


    private async Task<string> UploadFileAsync(FileInfo file)
    {
        var httpClient = await CreateAuthorizedHttpClient();
        var requestUrl = $"{_azureSettings.GraphEndpoint}root:/{file.Name}:/content";
        await using var stream = File.Open(file.FullName, FileMode.Open);
        var requestContent = new StreamContent(stream);
        requestContent.Headers.ContentType = new MediaTypeHeaderValue(MimeTypeMap.GetMimeType(file.Extension));
        var response = await httpClient.PutAsync(requestUrl, requestContent);
        var responseBody = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new AzureUploadFileException(
                $"Upload file failed with status {response.StatusCode} and message {responseBody}");
        dynamic fileResponse = JsonConvert.DeserializeObject(responseBody);
        if (fileResponse is null || fileResponse.id is null)
            throw new AzureUploadFileException($"Error deserializing response sourceFormat {requestUrl}");
        return fileResponse?.id;
    }

    private async Task<FileInfo> DownloadConvertedFileAsync(string directoryPath, string fileName, string fileId, string targetFormat)
    {
        var httpClient = await CreateAuthorizedHttpClient();
        var requestUrl = $"{_azureSettings.GraphEndpoint}{fileId}/content?format={targetFormat}";
        var response = await httpClient.GetAsync(requestUrl);
        if (!response.IsSuccessStatusCode)
            throw new AzureDownloadConvertedFileException(
                $"Downloading converted file failed with status {response.StatusCode} and message {response.Content.ReadAsStringAsync()}");
        await using var fileContent = await response.Content.ReadAsStreamAsync();
        var filePath = Path.Combine(directoryPath, fileName);
        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await fileContent.CopyToAsync(fileStream);
        return new FileInfo(filePath);
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

        using var client = new HttpClient();

        var requestUrl = $"{_azureSettings.AuthenticationEndpoint}{_azureSettings.TenantId}/oauth2/v2.0/token";
        var requestContent = new FormUrlEncodedContent(values);

        var response = await client.PostAsync(requestUrl, requestContent);
        var responseBody = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new AzureAuthorizationException($"Non-200 response sourceFormat {requestUrl}: {responseBody}");

        dynamic tokenResponse = JsonConvert.DeserializeObject(responseBody);
        if (tokenResponse is null || tokenResponse?.access_token is null)
            throw new AzureAuthorizationException($"Error deserializing response sourceFormat {requestUrl}");
        return tokenResponse?.access_token;
    }

    private async Task<HttpClient> CreateAuthorizedHttpClient()
    {
        if (_httpClient is not null) return _httpClient;

        var token = await GetAccessTokenAsync();
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        return _httpClient;
    }
}