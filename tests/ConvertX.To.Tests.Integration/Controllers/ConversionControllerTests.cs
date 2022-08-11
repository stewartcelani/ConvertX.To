using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Bogus;
using ConvertX.To.API.Contracts.V1;
using ConvertX.To.API.Contracts.V1.Mappers;
using ConvertX.To.API.Contracts.V1.Responses;
using ConvertX.To.Application.Converters;
using ConvertX.To.Application.Interfaces;
using ConvertX.To.Domain;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ConvertX.To.Tests.Integration.Controllers;

[ExcludeFromCodeCoverage]
[Collection(nameof(SharedTestCollection))]
public class ConversionControllerTests : IClassFixture<ConvertXToApiFactory>, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly IServiceScope _serviceScope;
    private readonly IConversionService _conversionService;
    private readonly Faker<Conversion> _conversionGenerator;

    public ConversionControllerTests(ConvertXToApiFactory apiFactory)
    {
        _httpClient = apiFactory.CreateClient();
        _serviceScope = apiFactory.Services.CreateScope();
        _conversionService = _serviceScope.ServiceProvider.GetRequiredService<IConversionService>();
        _conversionGenerator = SharedTestContext.ConversionGenerator;
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
    
    
    public void Dispose()
    {
        _httpClient.Dispose();
        _serviceScope.Dispose();
    }
}