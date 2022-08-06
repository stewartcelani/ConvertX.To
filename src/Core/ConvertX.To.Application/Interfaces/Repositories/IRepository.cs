using System.Linq.Expressions;
using ConvertX.To.Application.Domain.Entities.Common;
using ConvertX.To.Application.Domain.Filters;

namespace ConvertX.To.Application.Interfaces.Repositories;

public interface IRepository<TEntity, in TKey>
    where TEntity : BaseEntity<TKey>, IAggregateRoot
    where TKey : IEquatable<TKey>
{
    Task<bool> ExistsAsync(TKey id);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity?> GetAsync(TKey id, IEnumerable<string>? includeProperties = null);
    Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, IEnumerable<string>? includeProperties = null);

    Task<IEnumerable<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>>? predicate = null,
        IEnumerable<string>? includeProperties = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        PaginationFilter? paginationFilter = null);

    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null);
    Task<bool> CreateAsync(TEntity entity);
    Task<bool> CreateAsync(IEnumerable<TEntity> entities);
    Task<bool> UpdateAsync(TEntity entity);
    Task<bool> UpdateAsync(IEnumerable<TEntity> entities);
    Task<bool> DeleteAsync(TKey id);
    Task<bool> DeleteAsync(IEnumerable<TKey> ids);
}