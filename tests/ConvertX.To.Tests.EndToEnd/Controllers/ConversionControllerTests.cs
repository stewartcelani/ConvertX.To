using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ConvertX.To.API.Contracts.V1;
using ConvertX.To.API.Contracts.V1.Mappers;
using ConvertX.To.API.Contracts.V1.Responses;
using ConvertX.To.Application.Converters;
using FluentAssertions;

namespace ConvertX.To.Tests.EndToEnd.Controllers;

[Collection(nameof(SharedTestCollection))]
public class ConversionControllerTests : IClassFixture<ConvertXToApiFactory>, IDisposable
{
    private readonly HttpClient _httpClient;
    
    public ConversionControllerTests(ConvertXToApiFactory apiFactory)
    {
        _httpClient = apiFactory.CreateClient();
    }
    
    public void Dispose()
    {
        _httpClient.Dispose();
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
        var actualSupportedConversionsResponse = await response.Content.ReadFromJsonAsync<SupportedConversionsResponse>();
        actualSupportedConversionsResponse.Should().BeEquivalentTo(expectedSupportedConversionsResponse);
    }
    
    [Theory]
    [MemberData(nameof(GetSupportedConversions))]
    public async Task
        ConvertAsync_ShouldConvertFileToTargetFormat_AndDownloadConversion_WhenConversionIsSupported(
            string sourceFormat, string targetFormat)
    {
        // Arrange
        var sourceFile = SharedTestContext.GetSampleFilesForFormat(sourceFormat).MaxBy(x => x.Length);
        sourceFile.Should().NotBeNull($"a sample file for format {sourceFormat} should exist");
        var fileStreamContent = new StreamContent(sourceFile!.OpenRead());
        fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(SharedTestContext.GetMimeType(sourceFile.FullName));
        using var multipartFormContent = new MultipartFormDataContent();
        multipartFormContent.Add(fileStreamContent, "file", sourceFile.Name);

        // Act - Convert File
        var response = await _httpClient.PostAsync(ApiRoutesV1.Convert.Post.UrlFor(targetFormat), multipartFormContent);

        // Assert -- Convert File
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var conversionResponse = await response.Content.ReadFromJsonAsync<ConversionResponse>();
        conversionResponse!.SourceFormat.Should().Be(sourceFormat);
        conversionResponse!.TargetFormat.Should().Be(targetFormat);
        var id = Guid.Parse(conversionResponse!.Id);
        id.Should().NotBeEmpty();
        
        // Act -- Download File
        var downloadResponse = await _httpClient.GetAsync(ApiRoutesV1.Files.Get.UrlFor(id));
        
        // Assert - Download File
        downloadResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDirectory);
        var filePath = Path.Combine(tempDirectory, $"{id.ToString()}.{targetFormat}");
        await using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            await downloadResponse.Content.CopyToAsync(fs);
        }
        var fileInfo = new FileInfo(filePath);
        fileInfo.Exists.Should().BeTrue("the file should have been successfully saved");
        fileInfo.Extension.Should().Be($".{targetFormat}", "the file extension should match the target format");

        // Cleanup
        fileInfo.Delete();
        Directory.Delete(tempDirectory);
    }
    
    [Theory]
    [MemberData(nameof(GetSupportedConversionsToJpgWithIntermediatePdfConverter))]
    public async Task
        ConvertAsync_ShouldConvertFileToJpgZip_WithOneJpgPerPage_AndDownloadConversionAsZip_WhenConversionIsSupported(
            string sourceFormat, string targetFormat)
    {
        // Arrange
        var sourceFiles = SharedTestContext.GetSampleFilesForFormat(sourceFormat);
        var sourceFile = sourceFiles.Find(x => x.Name.Contains("pages", StringComparison.OrdinalIgnoreCase)) ?? sourceFiles.MaxBy(x => x.Length);
        sourceFile.Should().NotBeNull($"a sample file for format {sourceFormat} should exist");
        var fileStreamContent = new StreamContent(sourceFile!.OpenRead());
        fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(SharedTestContext.GetMimeType(sourceFile.FullName));
        using var multipartFormContent = new MultipartFormDataContent();
        multipartFormContent.Add(fileStreamContent, "file", sourceFile.Name);

        // Act - Convert File
        var response = await _httpClient.PostAsync(ApiRoutesV1.Convert.Post.UrlFor(targetFormat) + "?splitJpg=true", multipartFormContent);

        // Assert -- Convert File
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var conversionResponse = await response.Content.ReadFromJsonAsync<ConversionResponse>();
        conversionResponse!.SourceFormat.Should().Be(sourceFormat);
        conversionResponse!.TargetFormat.Should().Be(targetFormat);
        conversionResponse!.ConvertedFormat.Should().Be("zip");
        var id = Guid.Parse(conversionResponse!.Id);
        id.Should().NotBeEmpty();
        
        // Act -- Download File
        var downloadResponse = await _httpClient.GetAsync(ApiRoutesV1.Files.Get.UrlFor(id));
        
        // Assert - Download File
        downloadResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDirectory);
        var filePath = Path.Combine(tempDirectory, $"{id.ToString()}.zip");
        await using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            await downloadResponse.Content.CopyToAsync(fs);
        }
        var fileInfo = new FileInfo(filePath);
        fileInfo.Exists.Should().BeTrue("the file should have been successfully saved");
        fileInfo.Extension.Should().Be($".zip", "the file extension should be a zip file");

        // Cleanup
        fileInfo.Delete();
        Directory.Delete(tempDirectory);
    }
    
    public static IEnumerable<object[]>
        GetSupportedConversions()
    {
        var testParams = new List<object[]>();
        
        var supportedConversions = ConversionEngine.GetSupportedConversions();
        
        foreach (var sourceFormat in supportedConversions.SourceFormatTo.Keys)
        {
            var supportedTargetFormats = supportedConversions.SourceFormatTo[sourceFormat];
            testParams.AddRange(supportedTargetFormats.Select(supportedTargetFormat =>
                new object[] { sourceFormat, supportedTargetFormat }));
        }
        
        return testParams;
    }
    
    public static IEnumerable<object[]>
        GetSupportedConversionsToJpgWithIntermediatePdfConverter()
    {
        var testParams = new List<object[]>();
        
        var supportedConversions = ConversionEngine.GetSupportedConversions();
        
        if (!supportedConversions.TargetFormatFrom.ContainsKey("jpg"))
            throw new InvalidOperationException("The JPG format should be supported");
        
        var sourceFormats = supportedConversions.TargetFormatFrom["jpg"];
        
        foreach (var sourceFormat in sourceFormats.Where(x => ConversionEngine.IsConversionSupported(x, "pdf")))
        {
            testParams.Add(new object[] { sourceFormat, "jpg" });   
        }
        
        return testParams;
    }

   
}