namespace ConvertX.To.Application.Domain.Entities.Common;

public interface ISoftDelete
{
    public DateTimeOffset? DateDeleted { get; set; }
}