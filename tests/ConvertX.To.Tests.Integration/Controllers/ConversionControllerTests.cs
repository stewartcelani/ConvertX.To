using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Bogus;
using ConvertX.To.API.Contracts.V1;
using ConvertX.To.API.Contracts.V1.Mappers;
using ConvertX.To.API.Contracts.V1.Responses;
using ConvertX.To.Application.Converters;
using ConvertX.To.Application.Interfaces;
using ConvertX.To.Domain;
using ConvertX.To.Infrastructure.Shared.Services;
using ConvertX.To.Tests.Integration.Helpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ConvertX.To.Tests.Integration.Controllers;

[ExcludeFromCodeCoverage]
[Collection(nameof(SharedTestCollection))]
public class ConversionControllerTests : IClassFixture<ConvertXToApiFactory>, IDisposable
{
    private readonly Faker<Conversion> _conversionGenerator;
    private readonly IConversionService _conversionService;
    private readonly HttpClient _httpClient;
    private readonly MicrosoftGraphApiServer.MicrosoftGraphApiServer _microsoftGraphApiServer;
    private readonly IServiceScope _serviceScope;

    public ConversionControllerTests(ConvertXToApiFactory apiFactory, SharedTestContext testContext)
    {
        _httpClient = apiFactory.CreateClient();
        _serviceScope = apiFactory.Services.CreateScope();
        _conversionService = _serviceScope.ServiceProvider.GetRequiredService<IConversionService>();
        _conversionGenerator = SharedTestContext.ConversionGenerator;
        _microsoftGraphApiServer = testContext.MicrosoftGraphApiServer;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        _serviceScope.Dispose();
    }

    [Fact]
    public async Task GetSupportedConversions_ShouldReturnSupportedConversions_WhenAtLeastOneConverterExists()
    {
        // Arrange
        var supportedConversions = ConversionEngine.GetSupportedConversions();
        var expectedSupportedConversionsResponse = supportedConversions.ToSupportedConversionsResponse();

        // Act
        var response = await _httpClient.GetAsync(ApiRoutesV1.Convert.Get.Url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var supportedConversionsResponse = await response.Content.ReadFromJsonAsync<SupportedConversionsResponse>();
        supportedConversionsResponse.Should().BeEquivalentTo(expectedSupportedConversionsResponse);
    }

    /// <summary>
    /// Mocking the multi-part large file uploads the Graph SDK uses via WireMockServer isn't possible as the client
    /// uses a base address pointing to Microsoft Graph. Due to the fact we will need live E2E tests as relying on
    /// WireMockServer for the core part of the application I'll make sure to test all small + large file uploads
    /// for all formats there. Here we will just test the small file uploads which essentially passes through all the
    /// same code except for the upload part.
    /// </summary>
    [Theory]
    [MemberData(nameof(ConvertAsync_GetParamsForSampleFilesUnderLargeFileThreshold_WhenConversionIsSupported))]
    public async Task
        ConvertAsync_ShouldConvertFileToTargetFormat_WhenConversionIsSupportedAndFileIsUnderLargeFileThreshold(
            string sampleFileName, string targetFormat)
    {
        // Arrange
        var tempFileName = $"{Guid.NewGuid().ToString().Replace("-", "")}.{Path.GetExtension(sampleFileName)}";
        var sourceFile = SharedTestContext.GetSampleFile(sampleFileName);
        _microsoftGraphApiServer.SetupUploadFileAsyncEndpoint(tempFileName, sourceFile.OpenRead());
        await _microsoftGraphApiServer.SetupGetFileInTargetFormatAsyncEndpoint(sampleFileName, targetFormat);
        var fileStreamContent = new StreamContent(sourceFile.OpenRead());
        fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(SharedTestContext.GetMimeType(sampleFileName));
        using var multipartFormContent = new MultipartFormDataContent();
        multipartFormContent.Add(fileStreamContent, "file", sampleFileName);

        // Act
        var response = await _httpClient.PostAsync(ApiRoutesV1.Convert.Post.UrlFor(targetFormat), multipartFormContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var conversionResponse = await response.Content.ReadFromJsonAsync<ConversionResponse>();
        conversionResponse!.Id.Should().MatchRegex(RegexHelper.Guid);
    }

    private static IEnumerable<object[]>
        ConvertAsync_GetParamsForSampleFilesUnderLargeFileThreshold_WhenConversionIsSupported()
    {
        var testParams = new List<object[]>();

        var sampleFiles = SharedTestContext.GetSampleFilesUnderLargeFileThreshold();

        var supportedConversions = ConversionEngine.GetSupportedConversions();

        foreach (var sampleFile in sampleFiles)
        {
            var sampleFileExtension = sampleFile.Extension.Replace(".", "").ToLower();
            if (!supportedConversions.SourceFormatTo.ContainsKey(sampleFileExtension)) continue;
            var supportedTargetFormats = supportedConversions.SourceFormatTo[sampleFileExtension];
            testParams.AddRange(supportedTargetFormats.Select(supportedTargetFormat =>
                new object[] { sampleFile.Name, supportedTargetFormat }));
        }

        return testParams;
    }
}