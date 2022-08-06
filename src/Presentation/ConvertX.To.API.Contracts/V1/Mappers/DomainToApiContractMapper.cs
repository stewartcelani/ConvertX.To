using ConvertX.To.API.Contracts.V1.Responses;
using ConvertX.To.Application.Domain;
using Mapster;

namespace ConvertX.To.API.Contracts.V1.Mappers;

public static class DomainToApiContractMapperV1
{
    public static ConversionResponse ToConversionResponse(this Conversion conversion) =>
        conversion.Adapt<ConversionResponse>();

    public static SupportedConversionsResponse
        ToSupportedConversionsResponse(this SupportedConversions supportedConversions) =>
        supportedConversions.Adapt<SupportedConversionsResponse>();
}