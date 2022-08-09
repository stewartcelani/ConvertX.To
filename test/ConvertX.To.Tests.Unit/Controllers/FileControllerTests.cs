using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using ConvertX.To.API.Controllers.V1;
using ConvertX.To.Application.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace ConvertX.To.Tests.Unit.Controllers;

[ExcludeFromCodeCoverage]
public class FileControllerTests
{
    private readonly FileController _sut;
    private readonly IConversionService _conversionService = Substitute.For<IConversionService>();
    private readonly IConversionStorageService _conversionStorageService = Substitute.For<IConversionStorageService>();

    public FileControllerTests()
    {
        _sut = new FileController(_conversionService, _conversionStorageService);
    }

    [Fact]
    public async Task GetFileAsync_ShouldDownloadConvertedFile_WhenConversionExists()
    {
        // Arrange
        var conversionId = Guid.NewGuid();
        _conversionService.ExistsAsync(conversionId).Returns(true);
        var sampleFile = new FileInfo(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(),
            "../../../../SampleFiles/sample.doc")));
        _conversionStorageService.GetConvertedFile(conversionId).Returns(sampleFile.OpenRead());
        _conversionService.IncrementDownloadCounter(conversionId).Returns(true);

        // Act
        var result = (FileStreamResult)await _sut.GetFileAsync(conversionId);
        
        // Assert
        result.FileStream.Length.Should().Be(sampleFile.Length);
    }

    [Fact]
    public async Task GetFileAsync_ShouldReturnNotFound_WhenConversionDoesNotExistOrHasBeenSoftDeleted()
    {
        // Arrange
        var conversionId = Guid.NewGuid();
        _conversionService.ExistsAsync(conversionId).Returns(false);

        // Act
        var result = (NotFoundResult)await _sut.GetFileAsync(conversionId);
        
        // Assert
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task
        DeleteFileAsync_ShouldSoftDeleteConversionAndDeleteConvertedFile_WhenConversionExistsAndHasNotBeenSoftDeleted()
    {
        // Arrange
        var conversionId = Guid.NewGuid();
        _conversionService.ExistsAsync(conversionId).Returns(true);
        _conversionService.DeleteAsync(conversionId).Returns(true);

        // Act
        var result = (OkResult)await _sut.DeleteFileAsync(conversionId);

        // Assert
        result.StatusCode.Should().Be(200);
    }
    
    [Fact]
    public async Task
        DeleteFileAsync_ShouldReturnNotFound_WhenConversionDoesNotExistOrHasBeenSoftDeleted()
    {
        // Arrange
        var conversionId = Guid.NewGuid();
        _conversionService.ExistsAsync(conversionId).Returns(false);

        // Act
        var result = (NotFoundResult)await _sut.DeleteFileAsync(conversionId);
        
        // Assert
        result.StatusCode.Should().Be(404);
    }



}