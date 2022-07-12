namespace ConvertX.To.Domain.Common;

public abstract class BaseEntity<TKey> : BaseEntity
{
    public virtual TKey Id { get; set; }
}

public abstract class BaseEntity {}