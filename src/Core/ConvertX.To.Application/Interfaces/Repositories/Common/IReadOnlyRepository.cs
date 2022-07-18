using ConvertX.To.Domain.Common;

namespace ConvertX.To.Application.Interfaces.Repositories.Common;

public interface IReadOnlyRepository<TEntity, TKey> 
    where TEntity : BaseEntity<TKey>, IAggregateRoot
    where TKey : IEquatable<TKey>
{
    Task<TEntity> GetByIdAsync(TKey id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IEnumerable<TKey>> GetAllIdsAsync();
}