namespace ConvertX.To.Domain.Common;

public interface ISoftDelete
{
    public DateTimeOffset? DateDeleted { get; set; }

}