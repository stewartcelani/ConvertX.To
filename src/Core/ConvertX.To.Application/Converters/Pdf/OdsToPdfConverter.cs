using ConvertX.To.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.Application.Converters.Pdf;

public class OdsToPdfConverter : MsGraphDriveItemConverterBase
{
    public OdsToPdfConverter(IMsGraphFileConversionService msGraphFileConversionService, ILogger logger) : base("pdf", msGraphFileConversionService, logger)
    {
    }
}