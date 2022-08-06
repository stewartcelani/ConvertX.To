using ConvertX.To.API.Contracts.V1.Responses;
using ConvertX.To.Application.Domain;

namespace ConvertX.To.API.Contracts.V1.Mappers;

public static class DomainToApiContractMapperV1
{
    public static ConversionResponse ToConversionResponse(this Conversion conversion, int timeToLiveInMinutes)
    {
        return new ConversionResponse
        {
            Id = conversion.Id.ToString(),
            SourceFormat = conversion.SourceFormat,
            TargetFormat = conversion.TargetFormat,
            ConvertedFormat = conversion.ConvertedFormat,
            SourceMegabytes = conversion.SourceMegabytes,
            ConvertedMegabytes = conversion.ConvertedMegabytes,
            DateRequestReceived = conversion.DateRequestReceived,
            DateRequestCompleted = conversion.DateRequestCompleted,
            RequestSeconds =
                Math.Round((decimal)(conversion.DateRequestCompleted - conversion.DateRequestReceived).TotalSeconds, 2),
            DateScheduledForDeletion = DateTimeOffset.Now.AddMinutes(timeToLiveInMinutes)
        };
    }

    public static SupportedConversionsResponse ToSupportedConversionsResponse(
        this SupportedConversions supportedConversions)
    {
        return new SupportedConversionsResponse
        {
            TargetFormatFrom = supportedConversions.TargetFormatFrom,
            SourceFormatTo = supportedConversions.SourceFormatTo
        };
    }
}