using ConvertX.To.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.Application.Converters.Pdf;

public class EmlToPdfConverter : MsGraphDriveItemConverterBase
{
    public EmlToPdfConverter(IMsGraphFileConversionService msGraphFileConversionService, ILogger logger) : base("pdf", msGraphFileConversionService, logger)
    {
    }
}