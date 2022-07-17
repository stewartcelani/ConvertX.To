using ConvertX.To.Application.Exceptions;
using ConvertX.To.Application.Interfaces;
using ConvertX.To.Domain.Entities;
using ConvertX.To.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ConvertX.To.Infrastructure.Shared.Services;

/// <summary>
/// IConversionService is primarily concerned with DbSet<Conversion>
/// IConversionEngine is what actually handles the conversions
/// </summary>
public class ConversionService : IConversionService
{
    private readonly ApplicationDbContext _applicationDbContext;

    public ConversionService(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task CreateAsync(Conversion conversion)
    {
        _applicationDbContext.Conversions.Add(conversion);
        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Conversion conversion)
    {
        _applicationDbContext.Conversions.Update(conversion);
        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task ExpireConversions(int timeToLiveInMinutes)
    {
        var timeToLive = DateTimeOffset.Now.Subtract(TimeSpan.FromMinutes(timeToLiveInMinutes));
        var conversions =
            _applicationDbContext.Conversions
                .Where(x => x.DateDeleted == null 
                            && x.DateCreated < timeToLive)
                .ToList();
        var now = DateTimeOffset.Now;
        foreach (var conversion in conversions)
        {
            conversion.DateDeleted = now;
        }
        _applicationDbContext.Conversions.UpdateRange(conversions);
        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task<Conversion> GetByIdAsync(Guid conversionId)
    {
        try
        {
            return await _applicationDbContext.Conversions.FirstAsync(x => x.Id == conversionId);
        }
        catch (InvalidOperationException)
        {
            throw new ConversionNotFoundException($"Conversion with id {conversionId} not found in database.");
        }
    }

    /// <summary>
    /// Gets all non-expired conversions
    /// </summary>
    public List<Conversion> GetAll()
    {
        return _applicationDbContext.Conversions.Where(x => x.DateDeleted !=  null).ToList();
    }
    
    /// <summary>
    /// Gets a list of ids of non-expired conversions 
    /// </summary>
    public IEnumerable<Guid> GetAllIds()
    {
        return (from x in _applicationDbContext.Conversions
            where x.DateDeleted == null
            select x.Id).ToList();
    }
}