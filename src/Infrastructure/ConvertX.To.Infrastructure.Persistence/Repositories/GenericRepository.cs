using ConvertX.To.Application.Interfaces.Repositories.Common;
using ConvertX.To.Domain.Common;
using ConvertX.To.Infrastructure.Persistence.Contexts;

namespace ConvertX.To.Infrastructure.Persistence.Repositories;

public abstract class GenericRepository<TEntity, TKey> : GenericReadOnlyRepository<TEntity, TKey>, IRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>, IAggregateRoot
    where TKey : IEquatable<TKey>
{
    private readonly ApplicationDbContext _applicationDbContext;

    protected GenericRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public virtual void Add(TEntity item) => _applicationDbContext.Set<TEntity>().Add(item);

    public virtual void AddRange(IEnumerable<TEntity> items) => _applicationDbContext.Set<TEntity>().AddRange();
    public virtual void Update(TEntity item) => _applicationDbContext.Set<TEntity>().Update(item);

    public virtual void UpdateRange(IEnumerable<TEntity> items) =>
        _applicationDbContext.Set<TEntity>().UpdateRange(items);

    public virtual void Remove(TEntity item) => _applicationDbContext.Set<TEntity>().Remove(item);

    public virtual void RemoveRange(IEnumerable<TEntity> items) =>
        _applicationDbContext.Set<TEntity>().RemoveRange(items);

    public virtual async Task SaveChangesAsync() => await _applicationDbContext.SaveChangesAsync();
}