using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using MimeTypes.Core;
using Newtonsoft.Json;
using Xunit;

namespace ConvertX.To.Tests.Integration.MicrosoftGraphApiServer;

[ExcludeFromCodeCoverage]
[Collection(nameof(SharedTestCollection))]
public class MicrosoftGraphApiServerTests : IDisposable
{
    private readonly MicrosoftGraphApiServer _microsoftGraphApiServer;
    private readonly HttpClient _httpClient;
    
    public MicrosoftGraphApiServerTests(SharedTestContext testContext)
    {
        _httpClient = new HttpClient();
        _microsoftGraphApiServer = testContext.MicrosoftGraphApiServer;
    }

    [Fact]
    public async Task PingEndpoint_ShouldReturnOk_WhenConfiguredCorrectly()
    {
       // Act
        var response = await _httpClient.GetAsync($"{_microsoftGraphApiServer.Url}/ping");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AuthenticationEndpoint_ShouldReturnOk_WhenConfiguredCorrectly()
    {
        // Arrange
        var requestUrl = $"{_microsoftGraphApiServer.MsGraphSettings.AuthenticationEndpoint}/{_microsoftGraphApiServer.MsGraphSettings.TenantId}/oauth2/v2.0/token";
        var values = new List<KeyValuePair<string, string>>
        {
            new("client_id", _microsoftGraphApiServer.MsGraphSettings.ClientId),
            new("client_secret", _microsoftGraphApiServer.MsGraphSettings.ClientSecret),
            new("scope", _microsoftGraphApiServer.MsGraphSettings.Scope),
            new("grant_type", "client_credentials")
        };
        var requestContent = new FormUrlEncodedContent(values);
        
        // Act
        var response = await _httpClient.PostAsync(requestUrl, requestContent);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task UploadFileEndpoint_ShouldReturnCreated_WhenConfiguredCorrectly()
    {
        // 404: http://localhost:51923/root:/4f6b28f286974de79cf1b2497ae87752.doc:/content
        // 201: http://localhost:51923/root:/85385f0b39f042cb950c66b800ce972d..doc:/content
        // Arrange
        const string sampleFileName = "sample_pages.doc";
        var tempFileName = $"{Guid.NewGuid().ToString().Replace("-", "")}.{Path.GetExtension(sampleFileName)}";
        var sourceFile = SharedTestContext.GetSampleFile(sampleFileName);
        _microsoftGraphApiServer.SetupUploadFileAsyncEndpoint(tempFileName, sourceFile.OpenRead());
        var requestUrl = $"{_microsoftGraphApiServer.MsGraphSettings.GraphEndpoint}/root:/{tempFileName}:/content";
        var requestContent = new StreamContent(sourceFile.OpenRead());
        requestContent.Headers.ContentType = new MediaTypeHeaderValue(MimeTypeMap.GetMimeType(Path.GetExtension(sampleFileName)));
        
        // Act
        var response = await _httpClient.PutAsync(requestUrl, requestContent);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
    
    [Fact]
    public async Task GetFileInTargetFormatEndpoint_ShouldReturnOk_WhenConfiguredCorrectly()
    {
        // Arrange
        const string sampleFileName = "sample_pages.doc";
        await _microsoftGraphApiServer.SetupGetFileInTargetFormatAsyncEndpoint(sampleFileName, "pdf");
        var requestUrl = $"{_microsoftGraphApiServer.MsGraphSettings.GraphEndpoint}/01NFUP5G3ARY75TFOYMFDLWXZSQWVFX62R/content?format=pdf";
        
        // Act
        var response = await _httpClient.GetAsync(requestUrl);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task DeleteFileEndpoint_ShouldReturnNoContent_WhenConfiguredCorrectly()
    {
        // Arrange
        var requestUrl = $"{_microsoftGraphApiServer.MsGraphSettings.GraphEndpoint}/01NFUP5G3ARY75TFOYMFDLWXZSQWVFX62R";
        
        // Act
        var response = await _httpClient.DeleteAsync(requestUrl);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}