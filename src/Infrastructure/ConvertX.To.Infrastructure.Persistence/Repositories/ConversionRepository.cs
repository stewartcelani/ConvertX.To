using ConvertX.To.Application.Exceptions;
using ConvertX.To.Application.Interfaces.Repositories;
using ConvertX.To.Domain.Entities;
using ConvertX.To.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ConvertX.To.Infrastructure.Persistence.Repositories;

/// <summary>
/// This is just here as an example
/// I don't like the duplicating the overrides here and in ConversionRepository, multiple-inheritance would be nice
/// </summary>
public class ConversionRepository : GenericRepository<Conversion, Guid>, IConversionRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    
    public ConversionRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }
    
    public override async Task<Conversion> GetByIdAsync(Guid id) =>
        await base.GetByIdAsync(id) ?? throw new ConversionNotFoundException();
    
    public override async Task<IEnumerable<Conversion>> GetAllAsync() =>
        await _applicationDbContext.Conversions.Where(x => x.DateDeleted != null).ToListAsync();

    public override async Task<IEnumerable<Guid>> GetAllIdsAsync() => await
        (from conversion in _applicationDbContext.Conversions
            where conversion.DateDeleted == null
            select conversion.Id)
        .ToListAsync();
    
    public async Task<IEnumerable<Conversion>> GetOlderThanAsync(DateTimeOffset date) =>
        await _applicationDbContext.Conversions.Where(x => x.DateDeleted == null & x.DateCreated < date).ToListAsync();
}