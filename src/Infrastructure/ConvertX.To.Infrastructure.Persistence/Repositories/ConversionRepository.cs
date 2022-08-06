using ConvertX.To.Application.Domain.Entities;
using ConvertX.To.Application.Interfaces.Repositories;
using ConvertX.To.Infrastructure.Persistence.Contexts;

namespace ConvertX.To.Infrastructure.Persistence.Repositories;

public class ConversionRepository : GenericRepository<ConversionEntity, Guid>, IConversionRepository
{
    public ConversionRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public override Task<bool> UpdateAsync(ConversionEntity entity)
    {
        if (entity.Downloads == 0) DbContext.Entry(entity).Property(x => x.Downloads).IsModified = false;
        return base.UpdateAsync(entity);
    }

    public override Task<bool> UpdateAsync(IEnumerable<ConversionEntity> entities)
    {
        var conversionEntities = entities.ToList();
        foreach (var entity in conversionEntities.Where(entity => entity.Downloads == 0))
            DbContext.Entry(entity).Property(x => x.Downloads).IsModified = false;
        return base.UpdateAsync(conversionEntities);
    }

    public override async Task<bool> DeleteAsync(Guid id)
    {
        var conversionEntity = await GetAsync(id);
        if (conversionEntity is null) return false;
        conversionEntity.DateDeleted = DateTimeOffset.Now;
        return await UpdateAsync(conversionEntity);
    }

    public override async Task<bool> DeleteAsync(IEnumerable<Guid> ids)
    {
        var numberDeleted = 0;

        var conversionIds = ids.ToList();

        foreach (var id in conversionIds)
            if (await DeleteAsync(id))
                numberDeleted++;

        return numberDeleted == conversionIds.Count;
    }
}