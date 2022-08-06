namespace ConvertX.To.Application.Domain.Entities.Common;

public abstract class AuditableBaseEntity<TKey> : BaseEntity<TKey>, IAuditable
{
    public virtual DateTimeOffset DateCreated { get; set; } = DateTimeOffset.Now;
    public virtual DateTimeOffset? DateUpdated { get; set; }
}