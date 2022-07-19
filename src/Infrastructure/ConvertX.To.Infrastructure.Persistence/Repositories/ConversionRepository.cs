using ConvertX.To.Application.Exceptions;
using ConvertX.To.Application.Interfaces.Repositories;
using ConvertX.To.Domain.Entities;
using ConvertX.To.Infrastructure.Persistence.Contexts;

namespace ConvertX.To.Infrastructure.Persistence.Repositories;

public class ConversionRepository : GenericRepository<Conversion, Guid>, IConversionRepository
{
    
    public ConversionRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
    {
    }

    public override async Task<Conversion> GetByIdAsync(Guid id)
    {
        try
        {
            return await base.GetByIdAsync(id);
        }
        catch (InvalidOperationException ex)
        {
            throw new ConversionNotFoundException($"Conversion with {id.ToString()} not found in database", ex);
        }
    }

}