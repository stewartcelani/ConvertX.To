using ConvertX.To.Application.Domain.Entities;

namespace ConvertX.To.Application.Interfaces.Repositories;

public interface IConversionRepository : IRepository<ConversionEntity, Guid>
{
}