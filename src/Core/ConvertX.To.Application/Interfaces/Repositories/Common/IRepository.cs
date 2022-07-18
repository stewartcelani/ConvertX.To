using ConvertX.To.Domain.Common;

namespace ConvertX.To.Application.Interfaces.Repositories.Common;

public interface IRepository<TEntity, TKey> : IReadOnlyRepository<TEntity, TKey>, IWriteRepository<TEntity>
    where TEntity : BaseEntity<TKey>, IAggregateRoot
    where TKey : IEquatable<TKey>
{
}