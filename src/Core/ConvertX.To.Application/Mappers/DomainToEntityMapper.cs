using ConvertX.To.Application.Domain.Entities;
using ConvertX.To.Domain;

namespace ConvertX.To.Application.Mappers;

public static class DomainToEntityMapper
{
    public static ConversionEntity ToConversionEntity(this Conversion conversion)
    {
        return new ConversionEntity
        {
            Id = conversion.Id,
            SourceFormat = conversion.SourceFormat,
            TargetFormat = conversion.TargetFormat,
            ConvertedFormat = conversion.ConvertedFormat,
            SourceMegabytes = conversion.SourceMegabytes,
            ConvertedMegabytes = conversion.ConvertedMegabytes,
            DateRequestReceived = conversion.DateRequestReceived,
            DateRequestCompleted = conversion.DateRequestCompleted,
            RequestSeconds =
                Math.Round((decimal)(conversion.DateRequestCompleted - conversion.DateRequestReceived).TotalSeconds, 2)
        };
    }
}