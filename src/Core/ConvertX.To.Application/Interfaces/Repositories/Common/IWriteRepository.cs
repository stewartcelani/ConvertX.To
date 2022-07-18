using ConvertX.To.Domain.Common;

namespace ConvertX.To.Application.Interfaces.Repositories.Common;

public interface IWriteRepository<in TEntity> where TEntity : BaseEntity, IAggregateRoot
{
    void Add(TEntity item);
    void AddRange(IEnumerable<TEntity> items);
    void Update(TEntity item);
    void UpdateRange(IEnumerable<TEntity> items);
    void Remove(TEntity item);
    void RemoveRange(IEnumerable<TEntity> items);
    Task SaveChangesAsync();
}