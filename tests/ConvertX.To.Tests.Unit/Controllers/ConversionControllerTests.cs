using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using ConvertX.To.API.Contracts.V1;
using ConvertX.To.API.Contracts.V1.Mappers;
using ConvertX.To.API.Contracts.V1.Queries;
using ConvertX.To.API.Contracts.V1.Responses;
using ConvertX.To.API.Controllers.V1;
using ConvertX.To.API.Services;
using ConvertX.To.Application.Converters;
using ConvertX.To.Application.Domain.Settings;
using ConvertX.To.Application.Exceptions;
using ConvertX.To.Application.Extensions;
using ConvertX.To.Application.Interfaces;
using ConvertX.To.Domain;
using ConvertX.To.Domain.Options;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace ConvertX.To.Tests.Unit.Controllers;


public class ConversionControllerTests
{
    private readonly IConversionEngine _conversionEngine = Substitute.For<IConversionEngine>();
    private readonly IConversionService _conversionService = Substitute.For<IConversionService>();
    private readonly IConversionStorageService _conversionStorageService = Substitute.For<IConversionStorageService>();

    private readonly ILoggerAdapter<ConversionController> _logger =
        Substitute.For<ILoggerAdapter<ConversionController>>();

    private readonly ConversionController _sut;

    public ConversionControllerTests()
    {
        var uriService = new UriService("https://localhost");
        _sut = new ConversionController(_conversionService, _conversionEngine, _conversionStorageService, _logger,
            new ConversionLifecycleManagerSettings { TimeToLiveInMinutes = 30 }, uriService);
    }

    [Fact]
    public void GetSupportedConversions_ShouldReturnSupportedConversions_WhenConvertersExist()
    {
        // Arrange
        var supportedConversions = ConversionEngine.GetSupportedConversions();
        var supportedConversionsResponse = supportedConversions.ToSupportedConversionsResponse();

        // Act
        var result = (OkObjectResult)_sut.GetSupportedConversions();

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(supportedConversionsResponse);
    }

    [Fact]
    public async Task ConvertAsync_ShouldConvertFile_WhenRequestIsValid()
    {
        // Arrange
        const string fileNameWithoutExtension = "test";
        const string sourceFormat = "docx";
        const string targetFormat = "pdf";
        const string convertedFormat = "pdf";
        var file = SharedTestContext.GenerateFormFile($"{fileNameWithoutExtension}.{sourceFormat}", "Test content");
        var conversionOptionsQuery = new ConversionOptionsQuery();
        var convertedStream = new MemoryStream(new byte[10]);
        _conversionEngine.ConvertAsync(sourceFormat, targetFormat, Arg.Any<Stream>(), Arg.Any<ConversionOptions>())
            .Returns((convertedFormat, convertedStream));
        Conversion? conversion = null;
        _conversionService.GetConvertedFileName(fileNameWithoutExtension, targetFormat, convertedFormat)
            .Returns($"{fileNameWithoutExtension}.{convertedFormat}");
        _conversionService.CreateAsync(Arg.Do<Conversion>(x => conversion = x)).Returns(true);

        // Act
        var result = (CreatedResult)await _sut.ConvertAsync(targetFormat, file, conversionOptionsQuery);

        // Assert
        result.StatusCode.Should().Be(201);
        var conversionResponse = result.Value.As<ConversionResponse>();
        var expectedConversionResponse = conversion!.ToConversionResponse(30);
        expectedConversionResponse.DateScheduledForDeletion = conversionResponse.DateScheduledForDeletion;
        conversionResponse.Should().BeEquivalentTo(expectedConversionResponse);
        result.Location.Should().EndWith(ApiRoutesV1.Files.Get.UrlFor(conversion!.Id));
        _logger.Received(1).LogInformation("Conversion request: {sourceFormat} to {targetFormat}", sourceFormat,
            targetFormat);
    }

    [Fact]
    public async Task ConvertAsync_ShouldThrowInvalidFileLengthException_WhenFileLengthIsZero()
    {
        // Arrange
        const string fileNameWithoutExtension = "test";
        const string sourceFormat = "docx";
        const string targetFormat = "pdf";
        var file = SharedTestContext.GenerateFormFile($"{fileNameWithoutExtension}.{sourceFormat}", null);
        var conversionOptionsQuery = new ConversionOptionsQuery();

        // Act
        var action = async () => (CreatedResult)await _sut.ConvertAsync(targetFormat, file, conversionOptionsQuery);

        // Assert
        await action.Should().ThrowAsync<InvalidFileLengthException>();
    }

    [Fact]
    public async Task
        ConvertAsync_ShouldThrowUnsupportedConversionException_WhenNoConverterForSourceToTargetFormatExists()
    {
        // Arrange
        const string fileNameWithoutExtension = "test";
        const string sourceFormat = "pdf";
        const string targetFormat = "docx";
        var file = SharedTestContext.GenerateFormFile($"{fileNameWithoutExtension}.{sourceFormat}", "Test content");
        var conversionOptionsQuery = new ConversionOptionsQuery();
        var expectedExceptionMessage =
            $"Converting from {sourceFormat.Proper()} to {targetFormat.Proper()} is not supported.";
        _conversionEngine.ConvertAsync(sourceFormat, targetFormat, Arg.Any<Stream>(), Arg.Any<ConversionOptions>())
            .ThrowsAsync(new UnsupportedConversionException(expectedExceptionMessage));

        // Act
        var action = async () => (CreatedResult)await _sut.ConvertAsync(targetFormat, file, conversionOptionsQuery);

        // Assert
        await action.Should().ThrowAsync<UnsupportedConversionException>().WithMessage(expectedExceptionMessage);
    }

    [Fact]
    public async Task ConvertAsync_ShouldThrowApiException_WhenThereIsAnErrorSavingConversionToDatabase()
    {
        // Arrange
        const string fileNameWithoutExtension = "test";
        const string sourceFormat = "docx";
        const string targetFormat = "pdf";
        const string convertedFormat = "pdf";
        var file = SharedTestContext.GenerateFormFile($"{fileNameWithoutExtension}.{sourceFormat}", "Test content");
        var conversionOptionsQuery = new ConversionOptionsQuery();
        var convertedStream = new MemoryStream(new byte[10]);
        _conversionEngine.ConvertAsync(sourceFormat, targetFormat, Arg.Any<Stream>(), Arg.Any<ConversionOptions>())
            .Returns((convertedFormat, convertedStream));
        _conversionService.GetConvertedFileName(fileNameWithoutExtension, targetFormat, convertedFormat)
            .Returns($"{fileNameWithoutExtension}.{convertedFormat}");
        _conversionService.CreateAsync(Arg.Any<Conversion>()).Returns(false);
        const string expectedExceptionMessageWildcardPattern = "There was an unexpected error creating conversion:*";

        // Act
        var action = async () => (CreatedResult)await _sut.ConvertAsync(targetFormat, file, conversionOptionsQuery);

        // Assert
        await action.Should().ThrowAsync<ApiException>().WithMessage(expectedExceptionMessageWildcardPattern);
    }
}