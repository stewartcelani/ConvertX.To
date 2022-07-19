using ConvertX.To.Application.Interfaces.Repositories.Common;
using ConvertX.To.Domain.Entities;

namespace ConvertX.To.Application.Interfaces.Repositories;

public interface IConversionRepository : IRepository<Conversion, Guid>
{
}