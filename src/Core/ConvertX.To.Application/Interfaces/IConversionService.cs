using ConvertX.To.Application.Converters;
using ConvertX.To.Domain.Entities;

namespace ConvertX.To.Application.Interfaces;

public interface IConversionService
{
    Task<Conversion> GetByIdAsync(Guid conversionId);
    Task<IEnumerable<Conversion>> GetAllAsync();
    Task<IEnumerable<Guid>> GetAllIdsAsync();
    Task CreateAsync(Conversion conversion);
    Task UpdateAsync(Conversion conversion);
    Task ExpireConversions(int timeToLiveInMinutes);
}