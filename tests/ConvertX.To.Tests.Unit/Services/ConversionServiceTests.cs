using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bogus;
using ConvertX.To.Application.Domain;
using ConvertX.To.Application.Domain.Entities;
using ConvertX.To.Application.Domain.Filters;
using ConvertX.To.Application.Interfaces;
using ConvertX.To.Application.Interfaces.Repositories;
using ConvertX.To.Application.Mappers;
using ConvertX.To.Domain;
using ConvertX.To.Infrastructure.Shared.Services;
using FluentAssertions;
using Neleus.LambdaCompare;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;
using ValidationException = FluentValidation.ValidationException;

namespace ConvertX.To.Tests.Unit.Services;

[ExcludeFromCodeCoverage]
public class ConversionServiceTests
{
    private readonly ConversionService _sut;
    private readonly Faker<Conversion> _conversionGenerator;
    private readonly IConversionRepository _conversionRepository = Substitute.For<IConversionRepository>();
    
    

    public ConversionServiceTests()
    {
        _sut = new ConversionService(_conversionRepository);
        _conversionGenerator = SharedTestContext.ConversionGenerator;
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenConversionExists()
    {
        // Arrange
        var conversion = _conversionGenerator.Generate();
        _conversionRepository
            .ExistsAsync(Arg.Is<Expression<Func<ConversionEntity, bool>>>(expr =>
                Lambda.Eq(expr, x => x.Id == conversion.Id && x.DateDeleted == null))).Returns(true);

        // Act
        var result = await _sut.ExistsAsync(conversion.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenConversionDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _conversionRepository
            .ExistsAsync(Arg.Is<Expression<Func<ConversionEntity, bool>>>(expr =>
                Lambda.Eq(expr, x => x.Id == id && x.DateDeleted == null))).Returns(false);

        // Act
        var result = await _sut.ExistsAsync(id);

        // Assert
        result.Should().BeFalse();
    }


    [Fact]
    public async Task GetByIdAsync_ShouldReturnConversion_WhenConversionExists()
    {
        // Arrange
        var conversion = _conversionGenerator.Generate();
        var conversionEntity = conversion.ToConversionEntity();
        _conversionRepository.GetAsync(conversion.Id).Returns(conversionEntity);

        // Act
        var result = await _sut.GetByIdAsync(conversion.Id);

        // Assert
        result.Should().BeEquivalentTo(conversion);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenConversionDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _conversionRepository.GetAsync(id).ReturnsNull();

        // Act
        var result = await _sut.GetByIdAsync(id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnConversions_WhenConversionsExist()
    {
        // Arrange
        var conversions = _conversionGenerator.Generate(5);
        var conversionEntities = conversions.Select(x => x.ToConversionEntity()).ToList();
        var predicate = GetPredicateForConversionFilter(new ConversionFilter());
        _conversionRepository.GetManyAsync(
                Arg.Is<Expression<Func<ConversionEntity, bool>>?>(expr => Lambda.Eq(expr, predicate)),
                null, 
                Arg.Any<Func<IQueryable<ConversionEntity>, IOrderedQueryable<ConversionEntity>>?>())
            .Returns(conversionEntities);

        // Act
        var result = await _sut.GetAsync();

        // Assert
        result.Should().BeEquivalentTo(conversions);
    }
    
    [Fact]
    public async Task GetAsync_ShouldReturnEmptyList_WhenNoConversionsExist()
    {
        // Arrange
        var predicate = GetPredicateForConversionFilter(new ConversionFilter());
        _conversionRepository.GetManyAsync(
                Arg.Is<Expression<Func<ConversionEntity, bool>>?>(expr => Lambda.Eq(expr, predicate)),
                null, 
                Arg.Any<Func<IQueryable<ConversionEntity>, IOrderedQueryable<ConversionEntity>>?>())
            .Returns(Enumerable.Empty<ConversionEntity>());

        // Act
        var result = await _sut.GetAsync();

        // Assert
        result.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetAsync_ShouldReturnConversions_WhenConversionsMatchConversionFilter()
    {
        // Arrange
        var conversions = _conversionGenerator.Generate(10);
        var conversionEntities = conversions.Select(x => x.ToConversionEntity()).ToList();
        var random = new Random();
        foreach (var conversionEntity in conversionEntities.Where(_ => random.Next(0, 2) == 1))
        {
            conversionEntity.DateDeleted = DateTimeOffset.Now;
        }
        var deletedConversionEntities = conversionEntities.Where(x => x.DateDeleted != null).ToImmutableList();
        var deletedConversions = deletedConversionEntities.Select(x => x.ToConversion()).ToImmutableList();
        var conversionFilter = new ConversionFilter { Deleted = true };
        var predicate = GetPredicateForConversionFilter(conversionFilter);
        _conversionRepository.GetManyAsync(
                Arg.Is<Expression<Func<ConversionEntity, bool>>?>(expr => Lambda.Eq(expr, predicate)),
                null, 
                Arg.Any<Func<IQueryable<ConversionEntity>, IOrderedQueryable<ConversionEntity>>?>())
            .Returns(deletedConversionEntities);

        // Act
        var result = await _sut.GetAsync(conversionFilter);

        // Assert
        result.Should().BeEquivalentTo(deletedConversions);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnSubsetOfConversions_WhenMoreConversionsExistThanPaginationFilterRequests()
    {
        // Arrange
        var conversions = _conversionGenerator.Generate(10);
        var conversionEntities = conversions.Select(x => x.ToConversionEntity()).ToList();
        var pageSize = new Random().Next(conversions.Count);
        var conversionFilter = new ConversionFilter();
        var paginationFilter = new PaginationFilter { PageSize = pageSize };
        var predicate = GetPredicateForConversionFilter(new ConversionFilter());
        _conversionRepository.GetManyAsync(
                Arg.Is<Expression<Func<ConversionEntity, bool>>?>(expr => Lambda.Eq(expr, predicate)),
                null, 
                Arg.Any<Func<IQueryable<ConversionEntity>, IOrderedQueryable<ConversionEntity>>?>(),
                paginationFilter)
            .Returns(conversionEntities.Take(pageSize));

        // Act
        var result = await _sut.GetAsync(conversionFilter, paginationFilter);

        // Assert
        result.Should().BeEquivalentTo(conversions.Take(pageSize));
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateConversionAndReturnTrue_WhenConversionIsValid()
    {
        // Arrange
        var conversion = _conversionGenerator.Generate();
        var conversionEntity = conversion.ToConversionEntity();
        _conversionRepository.ExistsAsync(conversion.Id).Returns(false);
        _conversionRepository.CreateAsync(Arg.Do<ConversionEntity>(x => conversionEntity = x)).Returns(true);

        // Act
        var result = await _sut.CreateAsync(conversion);

        // Assert
        conversionEntity.Should().BeEquivalentTo(conversion);
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task CreateAsync_ShouldReturnFalse_WhenRepositoryCouldNotChangeDatabase()
    {
        // Arrange
        var conversion = _conversionGenerator.Generate();
        var conversionEntity = conversion.ToConversionEntity();
        _conversionRepository.ExistsAsync(conversion.Id).Returns(false);
        _conversionRepository.CreateAsync(Arg.Do<ConversionEntity>(x => conversionEntity = x)).Returns(false);

        // Act
        var result = await _sut.CreateAsync(conversion);

        // Assert
        conversionEntity.Should().BeEquivalentTo(conversion);
        result.Should().BeFalse();
    }
    
    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenConversionAlreadyExists()
    {
        // Arrange
        var conversion = _conversionGenerator.Generate();
        _conversionRepository.ExistsAsync(conversion.Id).Returns(true);
        var expectedExceptionMessage =  $"A conversion with id {conversion.Id} already exists";
        
        // Act
        var action = async () => await _sut.CreateAsync(conversion);

        // Assert
        await action.Should().ThrowAsync<ValidationException>().WithMessage(expectedExceptionMessage);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldUpdateConversionAndReturnTrue_WhenConversionIsValid()
    {
        // Arrange
        var conversion = _conversionGenerator.Generate();
        var conversionEntity = conversion.ToConversionEntity();
        _conversionRepository.ExistsAsync(conversion.Id).Returns(true);
        _conversionRepository.UpdateAsync(Arg.Do<ConversionEntity>(x => conversionEntity = x)).Returns(true);

        // Act
        var result = await _sut.UpdateAsync(conversion);

        // Assert
        conversionEntity.Should().BeEquivalentTo(conversion);
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenRepositoryCouldNotChangeDatabase()
    {
        // Arrange
        var conversion = _conversionGenerator.Generate();
        var conversionEntity = conversion.ToConversionEntity();
        _conversionRepository.ExistsAsync(conversion.Id).Returns(true);
        _conversionRepository.UpdateAsync(Arg.Do<ConversionEntity>(x => conversionEntity = x)).Returns(false);

        // Act
        var result = await _sut.UpdateAsync(conversion);

        // Assert
        conversionEntity.Should().BeEquivalentTo(conversion);
        result.Should().BeFalse();
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldThrowValidationException_WhenConversionDoesNotExist()
    {
        // Arrange
        var conversion = _conversionGenerator.Generate();
        _conversionRepository.ExistsAsync(conversion.Id).Returns(false);
        var expectedExceptionMessage =  $"Can not update conversion with id {conversion.Id} as it does not exist";
        
        // Act
        var action = async () => await _sut.UpdateAsync(conversion);

        // Assert
        await action.Should().ThrowAsync<ValidationException>().WithMessage(expectedExceptionMessage);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteConversion_WhenConversionExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        _conversionRepository.ExistsAsync(id).Returns(true);
        _conversionRepository.DeleteAsync(id).Returns(true);

        // Act
        var result = await _sut.DeleteAsync(id);

        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenRepositoryCouldNotChangeDatabase()
    {
        // Arrange
        var id = Guid.NewGuid();
        _conversionRepository.ExistsAsync(id).Returns(true);
        _conversionRepository.DeleteAsync(id).Returns(false);

        // Act
        var result = await _sut.DeleteAsync(id);

        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_FromGuardClause_WhenConversionDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _conversionRepository.ExistsAsync(id).Returns(false);

        // Act
        var result = await _sut.DeleteAsync(id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExpireConversions_ShouldExpireConversions_WhenConversionsMeetPredicate()
    {
        // Arrange
        var conversions = _conversionGenerator.Generate(5);
        var conversionEntities = conversions.Select(x => x.ToConversionEntity()).ToList();
        const int timeToLiveInMinutes = 30;
        var timeToLive = DateTimeOffset.Now.AddHours(2).Subtract(TimeSpan.FromMinutes(timeToLiveInMinutes));
        Expression<Func<ConversionEntity, bool>> predicate = x => (x.DateDeleted == null) & (x.DateCreated < timeToLive);
        var conversionEntitiesMatchingPredicate = conversionEntities.Where(predicate.Compile()).ToList();
        _conversionRepository
            .GetManyAsync(Arg.Is<Expression<Func<ConversionEntity, bool>>?>(expr => Lambda.Eq(expr, predicate)))
            .Returns(conversionEntitiesMatchingPredicate);
        // ReSharper disable twice PossibleMultipleEnumeration
        _conversionRepository.UpdateAsync(Arg.Is<IEnumerable<ConversionEntity>>(x => x.Count() == x.Select(conversionEntity => conversionEntity.DateDeleted != null).Count())).Returns(true);
        
        // Act
        var result = await _sut.ExpireConversions(timeToLive);

        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task IncrementDownloadCounter_ShouldIncreaseDownloadFieldByOne_WhenConversionExists()
    {
        // Arrange
        var conversion = _conversionGenerator.Generate();
        var conversionEntity = conversion.ToConversionEntity();
        _conversionRepository.GetAsync(conversion.Id).Returns(conversionEntity);
        var downloadsBeforeUpdate = conversionEntity.Downloads;
        conversionEntity.Downloads++;
        var downloadsAfterUpdate = conversionEntity.Downloads;
        _conversionRepository.UpdateAsync(conversionEntity).Returns(true);

        // Act
        var result = await _sut.IncrementDownloadCounter(conversion.Id);

        // Assert
        result.Should().BeTrue();
        downloadsBeforeUpdate.Should().Be(0);
        downloadsAfterUpdate.Should().Be(1);
    }

    [Fact]
    public async Task IncrementDownloadCounter_ShouldThrowValidationException_WhenConversionDoesNotExist()
    {
        // Arrange
        var conversion = _conversionGenerator.Generate();
        _conversionRepository.GetAsync(conversion.Id).ReturnsNull();
        var expectedExceptionMessage =  $"Can not increment download counter for conversion with id {conversion.Id} as it does not exist";
        
        // Act
        var action = async () => await _sut.IncrementDownloadCounter(conversion.Id);

        // Assert
        await action.Should().ThrowAsync<ValidationException>().WithMessage(expectedExceptionMessage);
    }
    
    [Fact]
    public async Task IncrementDownloadCounter_ShouldThrowValidationException_WhenConversionHasBeenSoftDeleted()
    {
        // Arrange
        var conversion = _conversionGenerator.Generate();
        var conversionEntity = conversion.ToConversionEntity();
        conversionEntity.DateDeleted = DateTimeOffset.Now;
        _conversionRepository.GetAsync(conversion.Id).Returns(conversionEntity);
        var expectedExceptionMessage =  $"Can not increment download counter for conversion with id {conversion.Id} as it has been deleted from disk";
        
        // Act
        var action = async () => await _sut.IncrementDownloadCounter(conversion.Id);

        // Assert
        await action.Should().ThrowAsync<ValidationException>().WithMessage(expectedExceptionMessage);
    }

    [Fact]
    public void GetConvertedFileName_ShouldReturnCorrectFileName_WhenTargetFormatAndConvertedFormatAreTheSame()
    {
        // Arrange
        const string fileNameWithoutExtension = "test";
        const string targetFormat = "pdf";
        const string convertedFormat = "pdf";

        // Act
        var result = _sut.GetConvertedFileName(fileNameWithoutExtension, targetFormat, convertedFormat);

        // Assert
        result.Should().Be($"{fileNameWithoutExtension}.{convertedFormat}");
    }
    
    [Fact]
    public void GetConvertedFileName_ShouldReturnCorrectFileName_WhenTargetFormatAndConvertedFormatAreDifferent()
    {
        // Arrange
        const string fileNameWithoutExtension = "test";
        const string targetFormat = "jpg";
        const string convertedFormat = "zip";

        // Act
        var result = _sut.GetConvertedFileName(fileNameWithoutExtension, targetFormat, convertedFormat);

        // Assert
        result.Should().Be($"{fileNameWithoutExtension}.{targetFormat}.{convertedFormat}");
    }
    
    private static Expression<Func<ConversionEntity, bool>> GetPredicateForConversionFilter(ConversionFilter conversionFilter)
    {
        Expression<Func<ConversionEntity, bool>>? predicate = x => x.DateDeleted == null;
        if (conversionFilter.Deleted) predicate = x => x.DateDeleted != null;
        return predicate;
    }


}