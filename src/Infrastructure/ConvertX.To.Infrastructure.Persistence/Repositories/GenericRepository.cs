using System.Linq.Expressions;
using ConvertX.To.Application.Interfaces.Repositories.Common;
using ConvertX.To.Domain.Common;
using ConvertX.To.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ConvertX.To.Infrastructure.Persistence.Repositories;

/// <summary>
/// // TODO: Repository Improvements:
/// - Implement generic IQueryable -- good (but old) reference I just noticed is
/// public class GenericRepository<TEntity> where TEntity : class -- at:
/// https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application
/// This will get rid of the need to override methods in the conversion repository for date deleted
/// </summary>
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

    public virtual void Add(TEntity item) => _dbSet.Add(item);

    public virtual void AddRange(IEnumerable<TEntity> items) => _dbSet.AddRange(items);
    public virtual void Update(TEntity item) => _dbSet.Update(item);

    public virtual void UpdateRange(IEnumerable<TEntity> items) =>
        _dbSet.UpdateRange(items);

    public virtual void Remove(TEntity item) => _dbSet.Remove(item);

    public virtual void RemoveRange(IEnumerable<TEntity> items) =>
        _dbSet.RemoveRange(items);

    public virtual async Task SaveChangesAsync() => await _applicationDbContext.SaveChangesAsync();
}