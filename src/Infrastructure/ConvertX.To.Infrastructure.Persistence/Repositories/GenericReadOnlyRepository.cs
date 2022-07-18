using ConvertX.To.Application.Interfaces.Repositories.Common;
using ConvertX.To.Domain.Common;
using ConvertX.To.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ConvertX.To.Infrastructure.Persistence.Repositories;

public abstract class GenericReadOnlyRepository<TEntity, TKey> : IReadOnlyRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>, IAggregateRoot 
    where TKey : IEquatable<TKey>
{
    private readonly ApplicationDbContext _applicationDbContext;

    protected GenericReadOnlyRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public virtual async Task<TEntity> GetByIdAsync(TKey id) =>
        await _applicationDbContext.Set<TEntity>().FirstAsync(x => x.Id.Equals(id));

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync() =>
        await _applicationDbContext.Set<TEntity>().ToListAsync();

    public virtual async Task<IEnumerable<TKey>> GetAllIdsAsync() => await
        (from x in _applicationDbContext.Set<TEntity>()
            select x.Id)
        .ToListAsync();
}