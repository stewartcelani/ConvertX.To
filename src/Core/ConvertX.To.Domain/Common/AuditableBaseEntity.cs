namespace ConvertX.To.Domain.Common;

public abstract class AuditableBaseEntity<TKey> : BaseEntity<TKey>, IAuditable
{
    public DateTimeOffset DateCreated { get; set; }
    public DateTimeOffset? DateUpdated { get; set; }
}