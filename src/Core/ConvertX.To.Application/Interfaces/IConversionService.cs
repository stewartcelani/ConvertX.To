using System.Linq.Expressions;
using ConvertX.To.Application.Domain;
using ConvertX.To.Application.Domain.Entities;
using ConvertX.To.Application.Domain.Filters;

namespace ConvertX.To.Application.Interfaces;

public interface IConversionService
{
    Task<Conversion?> GetByIdAsync(Guid id);
    Task<IEnumerable<Conversion>> GetAsync();
    Task<IEnumerable<Conversion>> GetAsync(ConversionFilter getCitiesFilter);
    Task<IEnumerable<Conversion>> GetAsync(ConversionFilter getCitiesFilter, PaginationFilter paginationFilter);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> CreateAsync(Conversion conversion);
    Task<bool> UpdateAsync(Conversion conversion);
    Task<bool> DeleteAsync(Guid conversionId);
    Task<bool> ExpireConversions(int timeToLiveInMinutes);
    Task<bool> IncrementDownloadCounter(Guid id);
    string GetConvertedFileName(string fileNameWithoutExtension, string targetFormat,
        string convertedFormat);
}