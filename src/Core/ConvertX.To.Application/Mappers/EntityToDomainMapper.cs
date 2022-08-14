using ConvertX.To.Application.Domain.Entities;
using ConvertX.To.Domain;

namespace ConvertX.To.Application.Mappers;

public static class EntityToDomainMapper
{
    public static Conversion ToConversion(this ConversionEntity conversionEntity)
    {
        return new Conversion
        {
            Id = conversionEntity.Id,
            SourceFormat = conversionEntity.SourceFormat,
            TargetFormat = conversionEntity.TargetFormat,
            ConvertedFormat = conversionEntity.ConvertedFormat,
            SourceMegabytes = conversionEntity.SourceMegabytes,
            ConvertedMegabytes = conversionEntity.ConvertedMegabytes,
            DateRequestReceived = conversionEntity.DateRequestReceived,
            DateRequestCompleted = conversionEntity.DateRequestCompleted
        };
    }
}