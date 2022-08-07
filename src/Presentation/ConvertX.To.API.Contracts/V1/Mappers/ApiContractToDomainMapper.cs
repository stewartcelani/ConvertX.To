using ConvertX.To.API.Contracts.V1.Queries;
using ConvertX.To.Domain.Options;

namespace ConvertX.To.API.Contracts.V1.Mappers;

public static class ApiContractToDomainMapperV1
{
    public static ConversionOptions ToConversionOptions(this ConversionOptionsQuery conversionOptionsQuery)
    {
        return new ConversionOptions
        {
            ToJpgOptions = new ToJpgOptions
            {
                SplitIfPossible = conversionOptionsQuery.SplitJpg
            }
        };
    }
}