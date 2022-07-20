namespace ConvertX.To.Application.Exceptions;

public interface IBusinessException
{
    // Marker interface, anything not IBusinessException will just return 500 with no content from API middleware
}