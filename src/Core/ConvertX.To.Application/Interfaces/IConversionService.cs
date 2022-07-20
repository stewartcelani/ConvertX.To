using System.Linq.Expressions;
using ConvertX.To.Application.Converters;
using ConvertX.To.Domain.Entities;

namespace ConvertX.To.Application.Interfaces;

public interface IConversionService
{
    Task<IEnumerable<Conversion>> GetAsync(Expression<Func<Conversion, bool>>? predicate = null,
        Func<IQueryable<Conversion>, IOrderedQueryable<Conversion>>? orderBy = null);
    Task<Conversion> GetByIdAsync(Guid conversionId);
    Task CreateAsync(Conversion conversion);
    Task UpdateAsync(Conversion conversion);
    Task ExpireConversions(int timeToLiveInMinutes);

    string GetConvertedFileName(string fileNameWithoutExtension, string targetFormat,
        string convertedFormat);
}