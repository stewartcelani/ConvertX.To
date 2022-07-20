using ConvertX.To.Domain.Common;

namespace ConvertX.To.Application.Interfaces.Repositories.Common;

public interface IWriteRepository<in TEntity> where TEntity : BaseEntity, IAggregateRoot
{
    Task CreateAsync(TEntity item);
    Task CreateAsync(IEnumerable<TEntity> items);
    Task UpdateAsync(TEntity item);
    Task UpdateAsync(IEnumerable<TEntity> items);
    Task DeleteAsync(TEntity item);
    Task DeleteAsync(IEnumerable<TEntity> items);
}