using System.Reflection;
using ConvertX.To.Application.Converters;
using ConvertX.To.Application.Exceptions.Business;
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
}