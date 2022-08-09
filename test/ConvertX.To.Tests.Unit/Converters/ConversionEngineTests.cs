using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ConvertX.To.Application.Converters;
using ConvertX.To.Application.Exceptions;
using ConvertX.To.Application.Extensions;
using ConvertX.To.Application.Interfaces;
using ConvertX.To.Domain.Options;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace ConvertX.To.Tests.Unit.Converters;

public class ConversionEngineTests
{
    private readonly ConversionEngine _sut;
    private readonly ILoggerAdapter<ConversionEngine> _logger = Substitute.For<ILoggerAdapter<ConversionEngine>>();
    private readonly ILoggerAdapter<IConverter> _converterLogger = Substitute.For<ILoggerAdapter<IConverter>>();

    private readonly ILoggerAdapter<ConverterFactory> _converterFactoryLogger =
        Substitute.For<ILoggerAdapter<ConverterFactory>>();

    private readonly IMsGraphFileConversionService _msGraphFileConversionService =
        Substitute.For<IMsGraphFileConversionService>();

    private readonly ConversionOptions _defaultConversionOptions;

    public ConversionEngineTests()
    {
        var converterFactory =
            new ConverterFactory(_converterFactoryLogger, _converterLogger, _msGraphFileConversionService);
        _sut = new ConversionEngine(_logger, converterFactory);
        _defaultConversionOptions = new ConversionOptions();
    }

    [Fact]
    public void GetSupportedConversions_ShouldReturnSupportedConversions_WhenConvertersExist()
    {
        // Act
        var result = ConversionEngine.GetSupportedConversions();

        // Assert
        result.SourceFormatTo.Count.Should().BePositive();
        result.TargetFormatFrom.Count.Should().BePositive();
    }

    [Fact]
    public async Task ConvertAsync_ShouldThrowUnsupportedConversionException_WhenConversionIsNotSupported()
    {
        // Arrange
        const string sourceFormat = "pdf";
        const string targetFormat = "docx";
        var stream = Stream.Null;
        var expectedExceptionMessage =
            $"Converting from {sourceFormat.Proper()} to {targetFormat.Proper()} is not supported.";

        // Act
        var action = async () =>
            await _sut.ConvertAsync(sourceFormat, targetFormat, stream, _defaultConversionOptions);

        // Assert
        await action.Should().ThrowAsync<UnsupportedConversionException>().WithMessage(expectedExceptionMessage);
    }

    [Theory]
    [MemberData(nameof(ConvertAsync_Data_SourceFormatToTargetFormatForAllConverters))]
    public async Task ConvertAsync_ShouldConvertFile_WhenConversionIsSupportedAndDefaultConversionOptionsAreUsed(string sourceFormat, string targetFormat)
    {
        // Arrange
        var source = new MemoryStream(new byte[2]);
        var convertedStream = new MemoryStream(new byte[1]);
        const string fileId = "01RFUB5G3LKSY7W5O3WRTY45I3L2DZ3UYZ";
        _msGraphFileConversionService.UploadFileAsync(sourceFormat, source).Returns(fileId);
        _msGraphFileConversionService.GetFileInTargetFormatAsync(fileId, targetFormat).Returns(convertedStream);

        // Act
        var (convertedFormatResult, convertedStreamResult) = await _sut.ConvertAsync(sourceFormat, targetFormat, source, _defaultConversionOptions);
        
        // Assert
        convertedFormatResult.Should().Be(targetFormat);
        convertedStreamResult.Length.Should().BePositive();
    }

    public static IEnumerable<object[]> ConvertAsync_Data_SourceFormatToTargetFormatForAllConverters()
    {
        // TODO: Use ConversionEngine ConversionEngine.GetSupportedConversions() to generate list of every source to target converter and return that
        // so that the above unit test runs through all converters. After that is done then figure out how to do the jpg/split ones.
        return new List<object[]>
        {
            new object[] { "docx", "pdf" }
        };
    } 
    
}