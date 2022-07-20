using System.Linq.Expressions;
using ConvertX.To.Application.Interfaces.Repositories.Common;
using ConvertX.To.Domain.Common;
using ConvertX.To.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ConvertX.To.Infrastructure.Persistence.Repositories;

public abstract class GenericRepository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>, IAggregateRoot
    where TKey : IEquatable<TKey>
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly DbSet<TEntity> _dbSet;

    protected GenericRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
        _dbSet = _applicationDbContext.Set<TEntity>();
    }
    
    public virtual async Task<IEnumerable<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null)
    {
        IQueryable<TEntity> query = _dbSet;

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return orderBy is null ? await query.ToListAsync() : await orderBy(query).ToListAsync();
    }
    
    public virtual async Task<TEntity> GetByIdAsync(TKey id) =>
        await _dbSet.FirstAsync(x => x.Id.Equals(id));

    public virtual async Task CreateAsync(TEntity item)
    {
        _dbSet.Add(item);
        await _applicationDbContext.SaveChangesAsync();
    }

    public virtual async Task CreateAsync(IEnumerable<TEntity> items)
    {
        _dbSet.AddRange(items);
        await _applicationDbContext.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(TEntity item)
    {
        _dbSet.Update(item);
        await _applicationDbContext.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(IEnumerable<TEntity> items)
    {
        _dbSet.UpdateRange(items);
        await _applicationDbContext.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(TEntity item)
    {
        _dbSet.Remove(item);
        await _applicationDbContext.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(IEnumerable<TEntity> items)
    {
        _dbSet.RemoveRange(items);
        await _applicationDbContext.SaveChangesAsync();
    }

}