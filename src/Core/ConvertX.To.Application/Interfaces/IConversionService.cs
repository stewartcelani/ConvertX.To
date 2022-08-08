using ConvertX.To.Application.Domain;
using ConvertX.To.Application.Domain.Filters;
using ConvertX.To.Domain;

namespace ConvertX.To.Application.Interfaces;

public interface IConversionService
{
    Task<bool> ExistsAsync(Guid id);
    Task<Conversion?> GetByIdAsync(Guid id);
    Task<IEnumerable<Conversion>> GetAsync();
    Task<IEnumerable<Conversion>> GetAsync(ConversionFilter conversionFilter);
    Task<IEnumerable<Conversion>> GetAsync(ConversionFilter conversionFilter, PaginationFilter paginationFilter);
    Task<bool> CreateAsync(Conversion conversion);
    Task<bool> UpdateAsync(Conversion conversion);
    Task<bool> DeleteAsync(Guid conversionId);
    Task<bool> ExpireConversions(DateTimeOffset timeToLive);
    Task<bool> IncrementDownloadCounter(Guid id);
    string GetConvertedFileName(string fileNameWithoutExtension, string targetFormat,
        string convertedFormat);
}