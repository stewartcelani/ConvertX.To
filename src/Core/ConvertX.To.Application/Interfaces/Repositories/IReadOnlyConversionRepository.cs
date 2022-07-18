using ConvertX.To.Application.Interfaces.Repositories.Common;
using ConvertX.To.Domain.Entities;

namespace ConvertX.To.Application.Interfaces.Repositories;

public interface IReadOnlyConversionRepository : IReadOnlyRepository<Conversion, Guid>
{
    Task<IEnumerable<Conversion>> GetOlderThanAsync(DateTimeOffset date);
}