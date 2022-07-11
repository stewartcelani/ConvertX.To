using ConvertX.To.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.Application.Converters.Pdf;

public class OdpToPdfConverter : MsGraphDriveItemConverterBase
{
    public OdpToPdfConverter(string sourceFormat, string targetFormat, ILogger logger, IMsGraphFileConversionService msGraphFileConversionService) : base(sourceFormat, targetFormat, logger, msGraphFileConversionService)
    {
    }
}