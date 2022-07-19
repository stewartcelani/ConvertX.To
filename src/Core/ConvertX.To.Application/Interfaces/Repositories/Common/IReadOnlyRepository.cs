using System.Linq.Expressions;
using ConvertX.To.Domain.Common;

namespace ConvertX.To.Application.Interfaces.Repositories.Common;

public interface IReadOnlyRepository<TEntity, in TKey> 
    where TEntity : BaseEntity<TKey>, IAggregateRoot
    where TKey : IEquatable<TKey>
{
    Task<IEnumerable<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null);
    
    Task<TEntity> GetByIdAsync(TKey id);
}