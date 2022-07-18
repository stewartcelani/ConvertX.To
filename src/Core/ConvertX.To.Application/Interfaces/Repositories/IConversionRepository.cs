using ConvertX.To.Domain.Entities;

namespace ConvertX.To.Application.Interfaces.Repositories;

public interface IConversionRepository : IReadOnlyConversionRepository, IWriteConversionRepository
{
}