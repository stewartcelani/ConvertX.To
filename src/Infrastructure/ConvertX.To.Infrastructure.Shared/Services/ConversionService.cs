using System.Linq.Expressions;
using ConvertX.To.Application.Domain;
using ConvertX.To.Application.Domain.Entities;
using ConvertX.To.Application.Domain.Filters;
using ConvertX.To.Application.Interfaces;
using ConvertX.To.Application.Interfaces.Repositories;
using ConvertX.To.Application.Mappers;
using ConvertX.To.Application.Validators.Helpers;
using ConvertX.To.Domain;
using FluentValidation;

namespace ConvertX.To.Infrastructure.Shared.Services;

public class ConversionService : IConversionService
{
    private readonly IConversionRepository _conversionRepository;

    public ConversionService(IConversionRepository conversionRepository)
    {
        _conversionRepository = conversionRepository;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _conversionRepository.ExistsAsync(x => x.Id == id && x.DateDeleted == null);
    }

    public async Task<Conversion?> GetByIdAsync(Guid id)
    {
        var conversionEntity = await _conversionRepository.GetAsync(id);
        return conversionEntity?.ToConversion();
    }
    
    public async Task<IEnumerable<Conversion>> GetAsync()
    {
        return await GetAsync(new ConversionFilter());
    }

    public async Task<IEnumerable<Conversion>> GetAsync(ConversionFilter conversionFilter)
    {
        var predicate = GetPredicateForConversionFilter(conversionFilter);

        var conversionEntities =
            await _conversionRepository.GetManyAsync(predicate, null,
                q => q.OrderByDescending(x => x.DateRequestCompleted));
        return conversionEntities.Select(x => x.ToConversion());
    }

    public async Task<IEnumerable<Conversion>> GetAsync(ConversionFilter conversionFilter,
        PaginationFilter paginationFilter)
    {
        var predicate = GetPredicateForConversionFilter(conversionFilter);

        var conversionEntities = await _conversionRepository.GetManyAsync(predicate, null,
            q => q.OrderByDescending(x => x.DateRequestCompleted), paginationFilter);
        return conversionEntities.Select(x => x.ToConversion());
    }

    
    public async Task<bool> CreateAsync(Conversion conversion)
    {
        if (await _conversionRepository.ExistsAsync(conversion.Id))
        {
            var message = $"A conversion with id {conversion.Id} already exists";
            throw new ValidationException(message, ValidationFailureHelper.Generate(nameof(Conversion), message));
        }

        var conversionEntity = conversion.ToConversionEntity();
        var created = await _conversionRepository.CreateAsync(conversionEntity);
        return created;
    }

    public async Task<bool> UpdateAsync(Conversion conversion)
    {
        if (!await _conversionRepository.ExistsAsync(conversion.Id))
        {
            var message = $"Can not update conversion with id {conversion.Id} as it does not exist";
            throw new ValidationException(message, ValidationFailureHelper.Generate(nameof(Conversion), message));
        }

        var conversionEntity = conversion.ToConversionEntity();
        var updated = await _conversionRepository.UpdateAsync(conversionEntity);
        return updated;
    }

    public async Task<bool> DeleteAsync(Guid conversionId)
    {
        if (!await _conversionRepository.ExistsAsync(conversionId)) return true;
        var deleted = await _conversionRepository.DeleteAsync(conversionId);
        return deleted;
    }

    public async Task<bool> ExpireConversions(DateTimeOffset timeToLive)
    {
        var conversions =
            (await _conversionRepository.GetManyAsync(x => (x.DateDeleted == null) & (x.DateCreated < timeToLive)))
            .ToList();
        var now = DateTimeOffset.Now;
        foreach (var conversion in conversions) conversion.DateDeleted = now;
        return await _conversionRepository.UpdateAsync(conversions);
    }

    public async Task<bool> IncrementDownloadCounter(Guid id)
    {
        var conversionEntity = await _conversionRepository.GetAsync(id);

        if (conversionEntity is null)
        {
            var message = $"Can not increment download counter for conversion with id {id} as it does not exist";
            throw new ValidationException(message, ValidationFailureHelper.Generate(nameof(Conversion), message));
        }

        if (conversionEntity!.DateDeleted is not null)
        {
            var message =
                $"Can not increment download counter for conversion with id {id} as it has been deleted from disk";
            throw new ValidationException(message, ValidationFailureHelper.Generate(nameof(Conversion), message));
        }

        conversionEntity.Downloads++;

        var updated = await _conversionRepository.UpdateAsync(conversionEntity);
        return updated;
    }

    public string GetConvertedFileName(string fileNameWithoutExtension, string targetFormat,
        string convertedFormat)
    {
        return targetFormat == convertedFormat
            ? $"{fileNameWithoutExtension}.{targetFormat}"
            : $"{fileNameWithoutExtension}.{targetFormat}.{convertedFormat}";
    }
    
    private static Expression<Func<ConversionEntity, bool>> GetPredicateForConversionFilter(ConversionFilter conversionFilter)
    {
        Expression<Func<ConversionEntity, bool>>? predicate = x => x.DateDeleted == null;
        if (conversionFilter.Deleted) predicate = x => x.DateDeleted != null;
        return predicate;
    }
}