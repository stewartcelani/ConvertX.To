using System.Linq.Expressions;
using ConvertX.To.Application.Interfaces;
using ConvertX.To.Application.Interfaces.Repositories;
using ConvertX.To.Domain.Entities;

namespace ConvertX.To.Infrastructure.Shared.Services;

/// <summary>
/// IConversionService is primarily concerned with DbSet<Conversion>
/// IConversionEngine is what actually handles the conversions
/// </summary>
public class ConversionService : IConversionService
{
    private readonly IConversionRepository _conversionRepository;

    public ConversionService(IConversionRepository conversionRepository)
    {
        _conversionRepository = conversionRepository;
    }

    public async Task<IEnumerable<Conversion>> GetAsync(Expression<Func<Conversion, bool>>? predicate = null,
        Func<IQueryable<Conversion>, IOrderedQueryable<Conversion>>? orderBy = null) =>
        await _conversionRepository.GetAsync(predicate, orderBy);
    
    public async Task<Conversion> GetByIdAsync(Guid conversionId) =>
        await _conversionRepository.GetByIdAsync(conversionId);

    public async Task CreateAsync(Conversion conversion)
    {
        await _conversionRepository.CreateAsync(conversion);
    }

    public async Task UpdateAsync(Conversion conversion)
    {
        await _conversionRepository.UpdateAsync(conversion);
    }

    public async Task ExpireConversions(int timeToLiveInMinutes)
    {
        var timeToLive = DateTimeOffset.Now.Subtract(TimeSpan.FromMinutes(timeToLiveInMinutes));
        var conversions =
            (await _conversionRepository.GetAsync(x => x.DateDeleted == null & x.DateCreated < timeToLive)).ToList();
        var now = DateTimeOffset.Now;
        foreach (var conversion in conversions)
        {
            conversion.DateDeleted = now;
        }
        await _conversionRepository.UpdateAsync(conversions);
    }
    
    public string GetConvertedFileName(string fileNameWithoutExtension, string targetFormat,
        string convertedFormat)
    {
        return targetFormat == convertedFormat
            ? $"{fileNameWithoutExtension}.{targetFormat}"
            : $"{fileNameWithoutExtension}.{targetFormat}.{convertedFormat}";
    }
}