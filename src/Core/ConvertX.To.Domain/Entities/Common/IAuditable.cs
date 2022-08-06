namespace ConvertX.To.Application.Domain.Entities.Common;

public interface IAuditable
{
    public DateTimeOffset DateCreated { get; set; }
    public DateTimeOffset? DateUpdated { get; set; }
}