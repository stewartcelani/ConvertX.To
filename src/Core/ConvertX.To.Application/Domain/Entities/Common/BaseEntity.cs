namespace ConvertX.To.Application.Domain.Entities.Common;

public abstract class BaseEntity<TKey> : BaseEntity
{
    public virtual TKey Id { get; set; }
}

public abstract class BaseEntity {}