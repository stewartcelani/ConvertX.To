using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
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
    private readonly ILoggerAdapter<ConverterFactory> _converterFactoryLogger =
        Substitute.For<ILoggerAdapter<ConverterFactory>>();

    private readonly ILoggerAdapter<IConverter> _converterLogger = Substitute.For<ILoggerAdapter<IConverter>>();

    private readonly ConversionOptions _defaultConversionOptions;
    private readonly ILoggerAdapter<ConversionEngine> _logger = Substitute.For<ILoggerAdapter<ConversionEngine>>();

    private readonly IMsGraphFileConversionService _msGraphFileConversionService =
        Substitute.For<IMsGraphFileConversionService>();

    private readonly ConversionEngine _sut;

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

    /// <summary>
    ///     This will test all converters with default conversion options
    /// </summary>
    [Theory]
    [MemberData(nameof(ConvertAsync_GetAllConverters_AsParams))]
    public async Task
        ConvertAsync_ShouldConvertFileAndReturnTargetFormat_WhenConversionIsSupportedAndDefaultConversionOptionsAreUsed(
            string sourceFormat, string targetFormat)
    {
        // Arrange
        var source = new MemoryStream(new byte[2]);
        var convertedStream = new MemoryStream(new byte[1]);
        const string fileId = "01RFUB5G3LKSY7W5O3WRTY45I3L2DZ3UYZ";
        _msGraphFileConversionService.UploadFileAsync(sourceFormat, source).Returns(fileId);
        _msGraphFileConversionService.GetFileInTargetFormatAsync(fileId, targetFormat).Returns(convertedStream);

        // Act
        var (convertedFormatResult, convertedStreamResult) =
            await _sut.ConvertAsync(sourceFormat, targetFormat, source, _defaultConversionOptions);

        // Assert
        convertedFormatResult.Should().Be(targetFormat);
        convertedStreamResult.Length.Should().BePositive();
    }

    /// <summary>
    ///     Tests all ToJpg converters that also have a ToPdf converter to use as an intermediary when
    ///     ConversionOptions -> ToJpgOptions -> SplitIfPossible = true
    /// </summary>
    [Theory]
    [MemberData(nameof(ConvertAsync_GetAllJpgConvertersSupportingMultiplePageSplitting_AsParams))]
    public async Task
        ConvertAsync_ShouldConvertFileAndReturnZipWithOneJpgPerPage_WhenSplitIfPossibleJpgOptionIsUsedAndAnIntermediaryPdfConverterExists(
            string sourceFormat, string targetFormat)
    {
        // Arrange
        var conversionOptions = new ConversionOptions
        {
            ToJpgOptions =
            {
                SplitIfPossible = true
            }
        };
        var sourceFile = SharedTestContext.GetSampleFile("sample_pages.doc");
        var sourceStream = sourceFile.OpenRead();
        var convertedFile = SharedTestContext.GetSampleFile("sample_pages.doc.converted.1_of_2.jpg");
        var convertedStream = convertedFile.OpenRead();
        var intermediaryPdfFile = SharedTestContext.GetSampleFile("sample_pages.pdf");
        var intermediaryPdfStream = intermediaryPdfFile.OpenRead();
        const string fileId = "01RFUB5G3LKSY7W5O3WRTY45I3L2DZ3UYZ";
        _msGraphFileConversionService.UploadFileAsync(Arg.Any<string>(), Arg.Any<Stream>()).Returns(fileId);
        _msGraphFileConversionService.GetFileInTargetFormatAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(convertedStream);
        _msGraphFileConversionService.GetFileInTargetFormatAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(intermediaryPdfStream);

        // Act
        var (convertedFormatResult, convertedStreamResult) =
            await _sut.ConvertAsync(sourceFormat, targetFormat, sourceStream, conversionOptions);

        // Assert
        convertedFormatResult.Should().Be("zip");
        convertedStreamResult.Length.Should().BePositive();

        // Cleanup
        await sourceStream.DisposeAsync();
        await convertedStream.DisposeAsync();
        await intermediaryPdfStream.DisposeAsync();
    }

    [Theory]
    [MemberData(nameof(ConvertAsync_GetAllJpgConvertersSupportingMultiplePageSplitting_AsParams))]
    public async Task
        ConvertAsync_ShouldConvertFileAndReturnJpg_WhenSplitIfPossibleJpgOptionIsUsedAndAnIntermediaryPdfConverterExistsAndOnlyOnePageExistsInSourceDocument(
            string sourceFormat, string targetFormat)
    {
        // Arrange
        var conversionOptions = new ConversionOptions
        {
            ToJpgOptions =
            {
                SplitIfPossible = true
            }
        };
        var sourceFile = SharedTestContext.GetSampleFile("sample_page.doc");
        var sourceStream = sourceFile.OpenRead();
        var convertedFile = SharedTestContext.GetSampleFile("sample_pages.doc.converted.1_of_2.jpg");
        var convertedStream = convertedFile.OpenRead();
        var intermediaryPdfFile = SharedTestContext.GetSampleFile("sample_page.pdf");
        var intermediaryPdfStream = intermediaryPdfFile.OpenRead();
        const string fileId = "01RFUB5G3LKSY7W5O3WRTY45I3L2DZ3UYZ";
        _msGraphFileConversionService.UploadFileAsync(Arg.Any<string>(), Arg.Any<Stream>()).Returns(fileId);
        _msGraphFileConversionService.GetFileInTargetFormatAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(convertedStream);
        _msGraphFileConversionService.GetFileInTargetFormatAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(intermediaryPdfStream);

        // Act
        var (convertedFormatResult, convertedStreamResult) =
            await _sut.ConvertAsync(sourceFormat, targetFormat, sourceStream, conversionOptions);

        // Assert
        convertedFormatResult.Should().Be(targetFormat);
        convertedStreamResult.Length.Should().BePositive();

        // Cleanup
        await sourceStream.DisposeAsync();
        await convertedStream.DisposeAsync();
        await intermediaryPdfStream.DisposeAsync();
    }

    private static IEnumerable<object[]> ConvertAsync_GetAllJpgConvertersSupportingMultiplePageSplitting_AsParams()
    {
        var supportedConversions = ConversionEngine.GetSupportedConversions();

        var toJpgConverters = supportedConversions.TargetFormatFrom["jpg"];

        var toPdfConverters = supportedConversions.TargetFormatFrom["pdf"];

        return (from sourceFormat in toJpgConverters
            where toPdfConverters.Contains(sourceFormat)
            select new object[] { sourceFormat, "jpg" }).ToList();
    }

    private static IEnumerable<object[]> ConvertAsync_GetAllConverters_AsParams()
    {
        var testParams = new List<object[]>();

        var supportedConversions = ConversionEngine.GetSupportedConversions();

        foreach (var (key, value) in supportedConversions.SourceFormatTo)
            testParams.AddRange(value.Select(s => new object[] { key, s }));

        return testParams;
    }
}