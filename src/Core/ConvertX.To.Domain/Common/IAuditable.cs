namespace ConvertX.To.Domain.Common;

public interface IAuditable
{
    public DateTimeOffset DateCreated { get; set; }
    public DateTimeOffset? DateUpdated { get; set; }
}