namespace ConvertX.To.API.Entities;

public interface IAuditable
{
    public DateTimeOffset DateCreated { get; set; }
    public DateTimeOffset? DateUpdated { get; set; }
}