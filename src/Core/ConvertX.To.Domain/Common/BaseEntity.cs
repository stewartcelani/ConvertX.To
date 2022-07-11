namespace ConvertX.To.Domain.Common;

public abstract class BaseEntity<TKey>
{
    public TKey Id { get; set; }
}