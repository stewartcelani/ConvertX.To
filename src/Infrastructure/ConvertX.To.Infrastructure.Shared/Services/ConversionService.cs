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

    public async Task CreateAsync(Conversion conversion)
    {
        _conversionRepository.Add(conversion);
        await _conversionRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(Conversion conversion)
    {
        _conversionRepository.Update(conversion);
        await _conversionRepository.SaveChangesAsync();
    }

    public async Task ExpireConversions(int timeToLiveInMinutes)
    {
        var timeToLive = DateTimeOffset.Now.Subtract(TimeSpan.FromMinutes(timeToLiveInMinutes));
        var conversions = (await _conversionRepository.GetOlderThanAsync(timeToLive)).ToList();
        var now = DateTimeOffset.Now;
        foreach (var conversion in conversions)
        {
            conversion.DateDeleted = now;
        }
        _conversionRepository.UpdateRange(conversions);
        await _conversionRepository.SaveChangesAsync();
    }

    public async Task<Conversion> GetByIdAsync(Guid conversionId) =>
        await _conversionRepository.GetByIdAsync(conversionId);

    /// <summary>
    /// Gets all non-expired/soft-deleted conversions
    /// </summary>
    public async Task<IEnumerable<Conversion>> GetAllAsync() => await _conversionRepository.GetAllAsync();

    /// <summary>
    /// Gets a list of ids of non-expired/soft-deleted conversions 
    /// </summary>
    public async Task<IEnumerable<Guid>> GetAllIdsAsync() => await _conversionRepository.GetAllIdsAsync();
}