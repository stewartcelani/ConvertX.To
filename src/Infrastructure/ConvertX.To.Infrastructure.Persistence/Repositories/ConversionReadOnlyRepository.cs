using ConvertX.To.Application.Exceptions;
using ConvertX.To.Application.Interfaces.Repositories;
using ConvertX.To.Domain.Entities;
using ConvertX.To.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ConvertX.To.Infrastructure.Persistence.Repositories;

public class ConversionReadOnlyRepository : GenericReadOnlyRepository<Conversion, Guid>, IReadOnlyConversionRepository
{

    private readonly ApplicationDbContext _applicationDbContext;

    public ConversionReadOnlyRepository(ApplicationDbContext applicationDbContext) :
        base(applicationDbContext)
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