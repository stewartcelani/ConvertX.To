namespace ConvertX.To.Application.Domain.Filters;

public class PaginationFilter
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 100;
}